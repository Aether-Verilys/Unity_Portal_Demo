using System.Text.RegularExpressions;
using UnityEngine;

public static class QuoteParserService
{
    [System.Serializable]
    private class QuoteJson
    {
        public bool isTradable;
        public string itemName;
        public int price;
        public string reason;
    }

    public static bool TryParse(string rawText, EraType era, out QuoteResult result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return false;
        }

        var json = ExtractJson(rawText);
        if (string.IsNullOrEmpty(json))
        {
            return false;
        }

        try
        {
            var parsed = JsonUtility.FromJson<QuoteJson>(json);
            if (parsed == null || (string.IsNullOrEmpty(parsed.itemName) && string.IsNullOrEmpty(parsed.reason)))
            {
                return false;
            }

            result = new QuoteResult
            {
                isTradable = parsed.isTradable,
                itemName = parsed.itemName,
                price = Mathf.Max(0, parsed.price),
                reason = parsed.reason,
                quotedEra = era
            };
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ExtractJson(string text)
    {
        var match = Regex.Match(text, "\\{[\\s\\S]*\\}");
        return match.Success ? match.Value : string.Empty;
    }
}
