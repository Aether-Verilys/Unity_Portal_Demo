using System.Collections.Generic;

public class AssetTierModel
{
    public AssetTierEntry EvaluateTier(List<AssetTierEntry> tiers, int totalAsset)
    {
        if (tiers == null || tiers.Count == 0)
        {
            return null;
        }

        AssetTierEntry matched = null;
        for (var i = 0; i < tiers.Count; i++)
        {
            var tier = tiers[i];
            if (tier != null && totalAsset >= tier.requiredAsset)
            {
                if (matched == null || tier.requiredAsset > matched.requiredAsset)
                {
                    matched = tier;
                }
            }
        }

        return matched;
    }
}
