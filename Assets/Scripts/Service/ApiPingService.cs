using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApiPingService : DRMSingleton<ApiPingService>
{
    private bool isPingInProgress;

    protected override void Awake()
    {
        base.Awake();
    }

    public async Task<PingResult> PingAsync(ApiRuntimeConfigSO config)
    {
        if (isPingInProgress)
        {
            return new PingResult { success = false, message = "Ping already in progress." };
        }

        if (config == null)
        {
            return new PingResult { success = false, message = "API configuration not found." };
        }

        var url = BuildUrl(config.baseUrl, config.apiPath);
        
        // Use POST with a minimal body because most AI endpoints (like /v1/chat/completions) do not support GET
        // We send a minimal request to check if the API key and URL are valid
        var dummyRequest = "{\"model\":\"" + config.modelName + "\",\"messages\":[{\"role\":\"user\",\"content\":\"ping\"}],\"max_tokens\":1}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(dummyRequest);

        isPingInProgress = true;

        try
        {
            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {config.apiKey}");

                var op = request.SendWebRequest();
                await AwaitAsyncOperation(op);

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    return new PingResult { success = false, message = "Connection failed, please check the URL." };
                }

                // 405 Method Not Allowed was likely because we used GET on an endpoint that only accepts POST (like OpenAI chat)
                if (request.responseCode >= 200 && request.responseCode < 300)
                {
                    return new PingResult { success = true, message = "API connection successful. You may start the game." };
                }

                if (request.responseCode == 401)
                {
                    return new PingResult { success = false, message = "API Key is invalid (401 Unauthorized)." };
                }

                if (request.responseCode == 404)
                {
                    return new PingResult { success = false, message = "Endpoint not found (404). Please check the Base URL and Path." };
                }

                return new PingResult { success = false, message = $"API response error: {request.responseCode}" };
            }
        }
        finally
        {
            isPingInProgress = false;
        }
    }

    private static Task AwaitAsyncOperation(AsyncOperation operation)
    {
        if (operation.isDone)
        {
            return Task.CompletedTask;
        }

        var completionSource = new TaskCompletionSource<bool>();
        operation.completed += _ => completionSource.TrySetResult(true);
        return completionSource.Task;
    }

    private string BuildUrl(string baseUrl, string path)
    {
        var left = (baseUrl ?? string.Empty).TrimEnd('/');
        var right = (path ?? string.Empty).TrimStart('/');
        return string.IsNullOrEmpty(right) ? left : $"{left}/{right}";
    }
}
