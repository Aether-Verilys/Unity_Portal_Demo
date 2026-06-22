[System.Serializable]
public class QuoteResult
{
    public bool isTradable;
    public string itemName;
    public int price;
    public string reason;
    public EraType quotedEra;
    public bool isSelling;

    public static QuoteResult Failure(string item, string failureReason)
    {
        return new QuoteResult
        {
            isTradable = false,
            itemName = item,
            price = 0,
            reason = failureReason,
            quotedEra = EraType.StoneAge
        };
    }
}
