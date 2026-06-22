using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TimeTrade/Config/GameBalance", fileName = "GameBalanceConfig")]
public class GameBalanceConfigSO : ScriptableObject
{
    public int startGold = 0;
    public int targetGold = 874296530;
    public int passiveGoldPerSecond = 1;
    public float quoteTimeoutSec = 15f;
    public bool requireApiConfigBeforeStart = true;
    public bool useFallbackWhenLLMFail = true;
    public List<AssetTierEntry> assetTiers = new List<AssetTierEntry>();
}
