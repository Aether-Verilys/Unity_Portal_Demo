using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TradeController : MonoBehaviour
{
    public TradePanel tradePanel;
    public GameLoopController gameLoopController;
    public EraController eraController;
    public LLMValuationService llmValuationService;

    public bool isSelling;
    public List<string> boughtItems = new List<string>();

    private QuoteResult _currentQuote;
    private bool _isRequesting;

    private void Awake()
    {
        if (llmValuationService == null)
        {
            llmValuationService = LLMValuationService.Instance;
        }

        if (gameLoopController == null)
        {
            gameLoopController = GameLoopController.Instance;
        }

        if (tradePanel != null)
        {
            tradePanel.OnSubmitItem += HandleSubmitItem;
            tradePanel.OnClickBuy += OnClickBuy;
            tradePanel.OnClickRetry += OnClickRetryItem;
            tradePanel.OnSaleToggleChanged += OnSaleToggleChanged;
        }

        if (eraController != null)
        {
            eraController.OnEraChanged += HandleEraChanged;
        }
    }

    private void OnDestroy()
    {
        if (tradePanel != null)
        {
            tradePanel.OnSubmitItem -= HandleSubmitItem;
            tradePanel.OnClickBuy -= OnClickBuy;
            tradePanel.OnClickRetry -= OnClickRetryItem;
            tradePanel.OnSaleToggleChanged -= OnSaleToggleChanged;
        }

        if (eraController != null)
        {
            eraController.OnEraChanged -= HandleEraChanged;
        }
    }

    private void HandleEraChanged(EraType era)
    {
        // 切换 era 时，sale 默认切换为买 (isSelling = false)
        if (tradePanel != null && tradePanel.saleToggle != null)
        {
            tradePanel.saleToggle.isOn = false;
        }

        // 数据同步：清除当前未完成的报价，因为时代变了
        _currentQuote = null;
        tradePanel?.HideDecision();
    }

    private void OnSaleToggleChanged(bool isSelling)
    {
        this.isSelling = isSelling;
    }

    private void Update()
    {
        if (_currentQuote == null || tradePanel == null || gameLoopController == null)
        {
            return;
        }

        tradePanel.SetBuyButtonInteractable(gameLoopController.CanBuyQuote(_currentQuote));
    }

    private async void HandleSubmitItem(string itemName)
    {
        await RequestQuoteAndTrade(itemName);
    }

    public async Task RequestQuoteAndTrade(string itemName)
    {
        if (_isRequesting)
        {
            return;
        }

        if (gameLoopController == null || !gameLoopController.IsRunning)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(itemName))
        {
            return;
        }

        _isRequesting = true;
        tradePanel?.SetSubmitButtonInteractable(false);
        try
        {
            
            var era = eraController == null ? EraType.StoneAge : eraController.CurrentEra;
            var quote = await llmValuationService.RequestQuoteAsync(itemName, era, isSelling);
            quote.quotedEra = era;

            _currentQuote = quote;
            tradePanel?.ShowQuote(quote);
            tradePanel?.ShowDecision(quote);

            var canBuy = gameLoopController.CanBuyQuote(quote);
            tradePanel?.SetBuyButtonInteractable(canBuy);
            
        }
        finally
        {
            _isRequesting = false;
            tradePanel?.SetSubmitButtonInteractable(true);
        }
    }

    public void OnClickBuy()
    {
        if (_currentQuote == null || gameLoopController == null)
        {
            return;
        }

        if (!gameLoopController.CanBuyQuote(_currentQuote))
        {
            tradePanel?.SetBuyButtonInteractable(false);
            return;
        }

        var success = gameLoopController.TryPurchaseQuote(_currentQuote);
        
        // 播放机器人随机动画
        Robot.Instance?.OnTransactionResult();

        if (success)
        {
            if (_currentQuote.isSelling)
            {
                // Retrieve selected items and remove them from boughtItems list
                var selected = tradePanel?.GetSelectedItemNames();
                if (selected != null)
                {
                    foreach (var name in selected)
                    {
                        boughtItems.Remove(name);
                    }
                    // 同步到 PlayerStateModel
                    gameLoopController.RemoveHeldItems(selected);
                }
                tradePanel?.RemoveSelectedItems();
            }
            else
            {
                boughtItems.Add(_currentQuote.itemName);
                tradePanel?.AddTradeItem(_currentQuote.itemName);
            }
            
            tradePanel?.HideDecision();
            tradePanel?.ClearInput();
            _currentQuote = null;
        }
        else
        {
        }
    }

    public void OnClickRetryItem()
    {
        _currentQuote = null;
        tradePanel?.HideDecision();
        tradePanel?.ClearInput();
    }
}
