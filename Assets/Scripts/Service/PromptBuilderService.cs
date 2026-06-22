using System.Text;

public static class PromptBuilderService
{
    public static string Build(string itemName, EraType era, int minPrice, int maxPrice, string eraInstruction, bool isSelling)
    {
        var builder = new StringBuilder();
        string persona = era == EraType.StoneAge ? "Tribal Chief" : "Corporate CEO";
        string eraName = era == EraType.StoneAge ? "Stone Age" : "Tech Era";

        if (era == EraType.StoneAge)
        {
            builder.AppendLine($"You are a powerful {persona} who evaluates items based on their contribution to the tribe's survival and dominance.");
        }
        else
        {
            builder.AppendLine($"You are a visionary {persona} of a global conglomerate, evaluating items based on high-tech efficiency, market potential, and strategic ROI.");
        }

        builder.AppendLine($"Currently, you are in the [{eraName}].");

        if (isSelling)
        {
            builder.AppendLine($"A customer wants to BUY this item from you: [{itemName}]");
            builder.AppendLine("You need to provide a selling price for this item.");
        }
        else
        {
            builder.AppendLine($"A customer wants to SELL this item to you: [{itemName}]");
            builder.AppendLine("You need to decide if you want to BUY it from them and at what price.");
        }

        builder.AppendLine($"According to current market conditions, the recommended price range for ordinary items is between {minPrice} and {maxPrice}.");
        
        builder.AppendLine("\n### YOUR EVALUATION CRITERIA ###");
        builder.AppendLine("1. **Utility and Scarcity**: Judge the item's utility in the current era. (e.g., a stone axe is invaluable in the Stone Age, but a museum curiosity in the Tech Era).");
        builder.AppendLine("2. **Logic and Taboos**: If the technology gap is too large to use or understand, consider it [Un-tradable in this era] with a value of 0.");
        builder.AppendLine("3. **Reasonable Pricing**: Provide a shrewd quote for tradable items; otherwise, the mandatory quote is 0.");
        builder.AppendLine("\n### CURRENT MARKET CONTEXT ###");
        builder.AppendLine(eraInstruction);

        builder.AppendLine("\n### OUTPUT FORMAT ###");
        builder.AppendLine("Please strictly adhere to the returned JSON format without any opening remarks or explanatory text. The format is as follows:");
        builder.AppendLine("{\"isTradable\":true,\"itemName\":\"Item Name\",\"price\":123,\"reason\":\"As the persona defined above, provide your professional pricing reason or reason for rejection in ONE or TWO sentences.\"}");
        
        return builder.ToString();
    }
}
