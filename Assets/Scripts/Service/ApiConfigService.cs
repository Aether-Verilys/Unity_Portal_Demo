using UnityEngine;

public class ApiConfigService : DRMSingleton<ApiConfigService>
{
    public ApiRuntimeConfigSO runtimeConfig;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Save(ApiConfigInput input)
    {
        if (runtimeConfig == null || input == null)
        {
            return;
        }

        runtimeConfig.baseUrl = input.baseUrl;
        runtimeConfig.apiPath = input.apiPath;
        runtimeConfig.modelName = input.modelName;
        runtimeConfig.apiKey = input.apiKey;
    }

    public void SetConfigured(bool configured)
    {
        if (runtimeConfig != null)
        {
            runtimeConfig.isConfigured = configured;
        }
    }

    public ApiRuntimeConfigSO Load()
    {
        return runtimeConfig;
    }

    public bool IsReady()
    {
        bool ready = runtimeConfig != null &&
               runtimeConfig.isConfigured &&
               !string.IsNullOrWhiteSpace(runtimeConfig.baseUrl) &&
               !string.IsNullOrWhiteSpace(runtimeConfig.modelName) &&
               !string.IsNullOrWhiteSpace(runtimeConfig.apiKey);

        if (!ready && runtimeConfig != null)
        {
            Debug.LogWarning($"ApiConfigService: Not Ready. isConfigured={runtimeConfig.isConfigured}, baseUrl={runtimeConfig.baseUrl}, model={runtimeConfig.modelName}, key={!string.IsNullOrWhiteSpace(runtimeConfig.apiKey)}");
        }
        else if (runtimeConfig == null)
        {
            Debug.LogWarning("ApiConfigService: runtimeConfig is null!");
        }

        return ready;
    }
}
