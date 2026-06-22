using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LLMValuationService : DRMSingleton<LLMValuationService>
{
    public ApiConfigService apiConfigService;
    public GameBalanceConfigSO gameBalanceConfig;
    public EraValuationHintConfigSO eraHintConfig;

    protected override void Awake()
    {
        base.Awake();
        if (apiConfigService == null)
        {
            apiConfigService = ApiConfigService.Instance;
        }
    }

    [System.Serializable]
    private class ChatMessage
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    private class ChatRequest
    {
        public string model;
        public List<ChatMessage> messages;
        public float temperature;
    }

    [System.Serializable]
    private class ChatChoice
    {
        public ChatMessage message;
    }

    [System.Serializable]
    private class ChatResponse
    {
        public List<ChatChoice> choices;
    }

    public async Task<QuoteResult> RequestQuoteAsync(string itemName, EraType era, bool isSelling)
    {
        if (apiConfigService == null)
        {
            apiConfigService = ApiConfigService.Instance;
        }

        if (string.IsNullOrWhiteSpace(itemName))
        {
            return QuoteResult.Failure(itemName, isSelling ? "Please select the items to sell" : "Please enter the name of the item to buy");
        }

        if (apiConfigService == null || !apiConfigService.IsReady())
        {
            return QuoteResult.Failure(itemName, "API is not configured");
        }

        var cfg = apiConfigService.Load();
        var minPrice = eraHintConfig == null ? 1 : eraHintConfig.minPrice;
        var maxPrice = eraHintConfig == null ? 100000 : eraHintConfig.maxPrice;
        var eraHint = string.Empty;
        if (eraHintConfig != null)
        {
            eraHint = era == EraType.StoneAge ? eraHintConfig.stoneAgeInstruction : eraHintConfig.techEraInstruction;
        }
        var prompt = PromptBuilderService.Build(itemName, era, minPrice, maxPrice, eraHint, isSelling);
        Debug.Log($"[LLM Prompt]:\n{prompt}");

        var req = new ChatRequest
        {
            model = cfg.modelName,
            messages = new List<ChatMessage>
            {
                new ChatMessage { role = "user", content = prompt }
            },
            temperature = 0.3f
        };

        var payload = JsonUtility.ToJson(req);
        var url = BuildUrl(cfg.baseUrl, cfg.apiPath);

        using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {cfg.apiKey}");
            request.timeout = Mathf.CeilToInt(gameBalanceConfig == null ? 15 : gameBalanceConfig.quoteTimeoutSec);

            var op = request.SendWebRequest();
            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                return BuildFallback(itemName, era, minPrice, maxPrice, "Request failed, using fallback valuation");
            }

            var raw = request.downloadHandler.text;
            Debug.Log($"[LLM Response RAW]:\n{raw}");

            // 1. Try parse as ChatResponse (API Wrapper)
            try
            {
                var response = JsonUtility.FromJson<ChatResponse>(raw);
                if (response != null && response.choices != null && response.choices.Count > 0)
                {
                    var content = response.choices[0].message.content;
                    Debug.Log($"[LLM Content]:\n{content}");

                    if (QuoteParserService.TryParse(content, era, out var parsedContent))
                    {
                        if (parsedContent.price > 0)
                        {
                            parsedContent.price = Mathf.Clamp(parsedContent.price, minPrice, maxPrice);
                        }
                        Debug.Log($"[LLM Content Parsed]: {parsedContent.price}");
                        parsedContent.isSelling = isSelling;
                        return parsedContent;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[LLM] ChatResponse parse failed: {ex.Message}");
            }

            // 2. Fallback: Try parse raw text directly (no wrapper)
            if (QuoteParserService.TryParse(raw, era, out var parsedDirect))
            {
                if (parsedDirect.price > 0)
                {
                    parsedDirect.price = Mathf.Clamp(parsedDirect.price, minPrice, maxPrice);
                }
                Debug.Log($"[LLM Direct Parsed]: {parsedDirect.price}");
                parsedDirect.isSelling = isSelling;
                return parsedDirect;
            }
        }

        var result = BuildFallback(itemName, era, minPrice, maxPrice, "Parsing failed, using fallback valuation");
        result.isSelling = isSelling;
        return result;
    }

    private QuoteResult BuildFallback(string itemName, EraType era, int minPrice, int maxPrice, string error)
    {
        if (gameBalanceConfig != null && !gameBalanceConfig.useFallbackWhenLLMFail)
        {
            return QuoteResult.Failure(itemName, error);
        }

        var fallback = FallbackPricingService.Quote(itemName, era, minPrice, maxPrice);
        fallback.reason = error;
        return fallback;
    }

    private string BuildUrl(string baseUrl, string path)
    {
        var left = (baseUrl ?? string.Empty).TrimEnd('/');
        var right = (path ?? string.Empty).TrimStart('/');
        return string.IsNullOrEmpty(right) ? left : $"{left}/{right}";
    }
}
