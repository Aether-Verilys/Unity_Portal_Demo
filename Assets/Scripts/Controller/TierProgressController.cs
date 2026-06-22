using UnityEngine;

public class TierProgressController : MonoBehaviour
{
    public GameBalanceConfigSO gameBalanceConfig;
    public ResourcesPanel resourcesPanel;

    private readonly AssetTierModel _assetTierModel = new AssetTierModel();
    private int _lastShownRequiredAsset = int.MinValue;

    public void EvaluateAndNotify(int totalAsset)
    {
        var tier = _assetTierModel.EvaluateTier(gameBalanceConfig == null ? null : gameBalanceConfig.assetTiers, totalAsset);
        if (tier == null)
        {
            return;
        }

        if (tier.requiredAsset == _lastShownRequiredAsset)
        {
            return;
        }

        _lastShownRequiredAsset = tier.requiredAsset;
        resourcesPanel?.ShowTierMilestone(tier.tierName, tier.milestoneText);
    }
}
