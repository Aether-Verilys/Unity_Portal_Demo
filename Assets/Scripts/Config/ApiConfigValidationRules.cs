public static class ApiConfigValidationRules
{
    public static bool Validate(ApiConfigInput input, out string error)
    {
        if (input == null)
        {
            error = "Configuration is null";
            return false;
        }

        if (string.IsNullOrWhiteSpace(input.baseUrl))
        {
            error = "Base URL cannot be empty";
            return false;
        }

        if (!input.baseUrl.StartsWith("http://") && !input.baseUrl.StartsWith("https://"))
        {
            error = "Base URL must start with http:// or https://";
            return false;
        }

        if (string.IsNullOrWhiteSpace(input.apiKey))
        {
            error = "API Key cannot be empty";
            return false;
        }

        if (string.IsNullOrWhiteSpace(input.modelName))
        {
            error = "Model name cannot be empty";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
