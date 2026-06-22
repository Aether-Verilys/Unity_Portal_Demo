using UnityEngine;

[CreateAssetMenu(menuName = "TimeTrade/Config/ApiRuntime", fileName = "ApiRuntimeConfig")]
public class ApiRuntimeConfigSO : ScriptableObject
{
    public string baseUrl;
    public string apiPath;
    public string modelName;
    public string apiKey;
    public bool useHttps = true;
    public bool isConfigured;
}
