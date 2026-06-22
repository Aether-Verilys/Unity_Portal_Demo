using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopController : DRMSingleton<GameLoopController>
{
    public GameBalanceConfigSO gameBalanceConfig;

    public ResourcesPanel resourcesPanel;
    public TutorialPanel tutorialPanel;
    public VictoryPanel victoryPanel;
    public TradePanel tradePanel;
    public TierProgressController tierProgressController;

    private readonly PlayerStateModel _playerState = new PlayerStateModel();
    private EconomyModel _economyModel;
    private readonly TimeIncomeService _timeIncomeService = new TimeIncomeService();
    private float _gameStartTime;
    
    public ApiConfigService apiConfigService;
    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }

    public int CurrentGold => _playerState.Gold;
    public List<ItemData> HeldItems => _playerState.HeldItems;
    public int CurrentTotalAsset => _playerState.CalcTotalAsset();

    protected override void Awake()
    {
        base.Awake();
        if (apiConfigService == null)
        {
            apiConfigService = ApiConfigService.Instance;
        }
    }

    private void Start()
    {
        // 初始刷新一次，确保目标金币等 UI 能显示出来
        RefreshResourcesUI();

        if (tutorialPanel != null)
        {
            IsRunning = false;
            tutorialPanel.OnClickStart += () => TryStartGame();
            tutorialPanel.gameObject.SetActive(true);
        }
        else
        {
            TryStartGame();
        }
    }

    public bool TryStartGame()
    {
        if (gameBalanceConfig == null)
        {
            return false;
        }

        if (gameBalanceConfig.requireApiConfigBeforeStart && (apiConfigService == null || !apiConfigService.IsReady()))
        {
            if (apiConfigService == null) Debug.LogError("GameLoopController: apiConfigService is null!");
            else if (!apiConfigService.IsReady()) Debug.LogError("GameLoopController: apiConfigService.IsReady() is false!");
            
            return false;
        }

        _playerState.Initialize(gameBalanceConfig.startGold);
        _playerState.OnGoldChanged += RefreshResourcesUI;
        _playerState.OnItemChanged += RefreshResourcesUI;
        
        // 当金币或物品变化时，也要检查胜利条件和进度更新
        _playerState.OnGoldChanged += CheckVictoryAndProgress;
        _playerState.OnItemChanged += CheckVictoryAndProgress;
        
        _economyModel = new EconomyModel(_playerState);
        _gameStartTime = Time.time;
        IsRunning = true;
        IsGameOver = false;
        RefreshResourcesUI();

        return true;
    }

    private void Update()
    {
        if (!IsRunning || IsGameOver || gameBalanceConfig == null)
        {
            return;
        }

        var income = _timeIncomeService.Tick(Time.deltaTime, gameBalanceConfig.passiveGoldPerSecond);
        if (income > 0)
        {
            _economyModel.AddGold(income);
        }

        // 任意时刻，如果金币满足条件就检查胜利
        CheckVictory();
    }

    private void CheckVictoryAndProgress()
    {
        if (!IsRunning || IsGameOver) return;

        tierProgressController?.EvaluateAndNotify(CurrentTotalAsset);
        CheckVictory();
    }

    public void CheckVictory()
    {
        if (gameBalanceConfig == null)
        {
            return;
        }

        // 将胜利判定从总资产改为拥有的金币
        if (CurrentGold >= gameBalanceConfig.targetGold)
        {
            TriggerVictory(CurrentGold);
        }
    }

    public void TriggerVictory(int totalAsset)
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        IsRunning = false;
        
        float elapsed = Time.time - _gameStartTime;

        if (victoryPanel == null)
        {
            victoryPanel = FindObjectOfType<VictoryPanel>(true);
        }

        if (victoryPanel != null)
        {
            victoryPanel.gameObject.SetActive(true);
            victoryPanel.ShowVictory(totalAsset, elapsed);
        }
        else
        {
            Debug.LogError("GameLoopController: victoryPanel not found in scene!");
        }
        
        EndGame();
    }

    public void EndGame()
    {
        tradePanel?.SetInteractable(false);
        victoryPanel?.LockGameplayInput();
    }

    public bool CanBuyQuote(QuoteResult quote)
    {
        if (quote == null || !quote.isTradable || _economyModel == null)
        {
            return false;
        }

        if (quote.isSelling)
        {
            return true;
        }

        // 移除 _playerState.CanHoldNewItem() 检查，只检查是否有足够的钱
        return _economyModel.CanPay(quote.price);
    }

    public bool TryPurchaseQuote(QuoteResult quote)
    {
        if (!IsRunning || IsGameOver || _economyModel == null)
        {
            return false;
        }

        return _economyModel.ApplyPurchase(quote);
    }

    public bool TrySellHeldItem()
    {
        if (!IsRunning || IsGameOver || _economyModel == null)
        {
            return false;
        }

        return _economyModel.TrySellHeldItem();
    }

    public void RemoveHeldItems(List<string> itemNames)
    {
        _playerState.RemoveHeldItems(itemNames);
    }

    public void RefreshResourcesUI()
    {
        if (resourcesPanel == null)
        {
            resourcesPanel = FindObjectOfType<ResourcesPanel>(true);
        }

        if (victoryPanel == null)
        {
            victoryPanel = FindObjectOfType<VictoryPanel>(true);
        }

        if (resourcesPanel == null)
        {
            return;
        }

        int targetGold = gameBalanceConfig != null ? gameBalanceConfig.targetGold : 0;
        resourcesPanel.RefreshGold(_playerState.Gold, targetGold);
        
        string heldText = string.Empty;
        if (_playerState.HeldItems.Count > 0)
        {
            heldText = _playerState.HeldItems.Count == 1 ? _playerState.HeldItems[0].itemName : $"{_playerState.HeldItems.Count} Items";
        }
        resourcesPanel.RefreshHeldItem(heldText);
    }
}
