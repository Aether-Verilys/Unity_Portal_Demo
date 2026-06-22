using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApiConfigPanel : MonoBehaviour
{
    public TMP_InputField baseUrlInput;
    public TMP_InputField apiPathInput;
    public TMP_InputField modelInput;
    public TMP_InputField apiKeyInput;
    public Button pingBtn;
    public Button backBtn;
    public TMP_Text statusText;

    public ApiConfigService apiConfigService;
    public ApiPingService apiPingService;

    public event Action OnClickBack;
    public event Action OnConfigChanged;

    private void Awake()
    {
        if (pingBtn != null)
        {
            pingBtn.onClick.AddListener(HandleClickTest);
        }

        if (backBtn != null)
        {
            backBtn.onClick.AddListener(() => OnClickBack?.Invoke());
        }
    }

    private void OnDestroy()
    {
        if (pingBtn != null)
        {
            pingBtn.onClick.RemoveListener(HandleClickTest);
        }

        if (backBtn != null)
        {
            backBtn.onClick.RemoveAllListeners();
        }
    }

    public ApiConfigInput ReadInput()
    {
        return new ApiConfigInput
        {
            baseUrl = baseUrlInput == null ? string.Empty : baseUrlInput.text.Trim(),
            apiPath = apiPathInput == null ? string.Empty : apiPathInput.text.Trim(),
            modelName = modelInput == null ? string.Empty : modelInput.text.Trim(),
            apiKey = apiKeyInput == null ? string.Empty : apiKeyInput.text.Trim()
        };
    }

    public void ShowValidateError(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void ShowPingResult(bool success, string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
        if (visible && statusText != null)
        {
            statusText.text = string.Empty;
        }
    }

    private void Start()
    {
        if (apiConfigService == null)
        {
            apiConfigService = ApiConfigService.Instance;
        }

        if (apiPingService == null)
        {
            apiPingService = ApiPingService.Instance;
        }
    }

    private async void HandleClickTest()
    {
        var input = ReadInput();
        if (!ApiConfigValidationRules.Validate(input, out var error))
        {
            ShowValidateError(error);
            return;
        }

        if (apiConfigService != null)
        {
            apiConfigService.Save(input);
        }

        if (apiPingService != null)
        {
            statusText.text = "Pinging...";
            pingBtn.interactable = false;
            var pingResult = await apiPingService.PingAsync(apiConfigService.Load());
            apiConfigService.SetConfigured(pingResult.success);
            ShowPingResult(pingResult.success, pingResult.message);
            pingBtn.interactable = true;
            
            if (pingResult.success)
            {
                OnConfigChanged?.Invoke();
            }
        }
    }

    public void Prefill(ApiRuntimeConfigSO config)
    {
        if (config == null) return;
        if (baseUrlInput != null) baseUrlInput.text = config.baseUrl;
        if (apiPathInput != null) apiPathInput.text = config.apiPath;
        if (modelInput != null) modelInput.text = config.modelName;
        if (apiKeyInput != null) apiKeyInput.text = config.apiKey;
    }
}
