using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerStateModel
{
    public int Gold { get; private set; }
    public List<ItemData> HeldItems { get; private set; } = new List<ItemData>();

    public event Action OnGoldChanged;
    public event Action OnItemChanged;

    public void Initialize(int startGold)
    {
        Gold = startGold;
        HeldItems.Clear();
        OnGoldChanged?.Invoke();
        OnItemChanged?.Invoke();
    }

    public bool CanHoldNewItem()
    {
        return true; // 移除“只能持有1个物体”的限制
    }

    public void AddHeldItem(ItemData item)
    {
        HeldItems.Add(item);
        OnItemChanged?.Invoke();
    }

    public void RemoveHeldItems(List<string> itemNames)
    {
        foreach (var name in itemNames)
        {
            var item = HeldItems.FirstOrDefault(i => i.itemName == name);
            if (item != null)
            {
                HeldItems.Remove(item);
            }
        }
        OnItemChanged?.Invoke();
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke();
    }

    public bool SpendGold(int amount)
    {
        if (amount < 0 || Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        OnGoldChanged?.Invoke();
        return true;
    }

    public int CalcTotalAsset()
    {
        var heldValue = HeldItems.Sum(item => item.lastQuotedPrice);
        return Gold + heldValue;
    }
}
