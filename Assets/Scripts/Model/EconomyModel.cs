public class EconomyModel
{
    private readonly PlayerStateModel _player;

    public EconomyModel(PlayerStateModel player)
    {
        _player = player;
    }

    public bool CanPay(int price)
    {
        return price >= 0 && _player.Gold >= price;
    }

    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            _player.AddGold(amount);
        }
    }

    public bool ApplyPurchase(QuoteResult quote)
    {
        if (quote == null || !quote.isTradable)
        {
            return false;
        }

        if (quote.isSelling)
        {
            _player.AddGold(quote.price);
            // 注意：这里由 Controller 处理具体移除了哪些物品
            return true;
        }

        if (!_player.SpendGold(quote.price))
        {
            return false;
        }

        _player.AddHeldItem(new ItemData
        {
            itemName = quote.itemName,
            lastQuotedPrice = quote.price,
            pricedEra = quote.quotedEra
        });
        return true;
    }

    public bool TrySellHeldItem()
    {
        if (_player.HeldItems.Count == 0)
        {
            return false;
        }

        // 这个方法可能需要废弃或者改为卖掉所有物品，
        // 目前 TradeController 已经自己处理了更精细的逻辑。
        return false;
    }
}
