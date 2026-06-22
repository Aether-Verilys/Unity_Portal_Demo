using UnityEngine;

public static class FallbackPricingService
{
    public static QuoteResult Quote(string itemName, EraType era, int minPrice, int maxPrice)
    {
        var hash = Mathf.Abs(itemName == null ? 1 : itemName.GetHashCode());
        var normalized = minPrice + (hash % Mathf.Max(1, maxPrice - minPrice + 1));

        var eraMultiplier = era == EraType.StoneAge ? 1.5f : 0.8f;
        var rawPrice = Mathf.RoundToInt(normalized * eraMultiplier);
        var price = Mathf.Clamp(rawPrice, minPrice, maxPrice);

        return new QuoteResult
        {
            isTradable = true,
            itemName = itemName,
            price = price,
            reason = "Network exception, fallback valuation used.",
            quotedEra = era
        };
    }
}
