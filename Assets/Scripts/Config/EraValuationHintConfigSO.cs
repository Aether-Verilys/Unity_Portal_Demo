using UnityEngine;

[CreateAssetMenu(menuName = "TimeTrade/Config/EraValuationHint", fileName = "EraValuationHintConfig")]
public class EraValuationHintConfigSO : ScriptableObject
{
    [TextArea(10, 25)]
    public string stoneAgeInstruction = @"[Stone Age - Survival and Shamanic Environment]
- Core Trade Logic: 'Survival is Life'. Anything that extends life, reduces pain, or provides food is a treasure.
- Legendary Weapons: Any metal products (stainless steel pots, kitchen knives, even paperclips) are considered 'Star-fallen Iron', with power exceeding tribal authority; prices should be extremely high.
- Survival Miracles: Cooked eggs, preserved food, and clean salt are dream luxuries for Stone Age people suffering from parasites and hunger.
- Shamanic Items: Glowing electronics (flashlights) are seen as 'Sun-trapped Sorcery'. High value, but plummet once power is lost.
- Fire & Tools: Fire can be produced locally, so common torches are not rare. However, *Lighters* are extremely precious as they create fire instantly and reliably, seen as a divine convenience.
- Negative Value (Price 0): Logic-defying advanced products like programming books (paper is too fragile to wrap things), CPU chips (too small and hard to process), or cosmetics (seen as evil blood).
- Special Setting: Local products like 'Giant Beast Eggs' are top-tier food if fresh, but ignored if fossilized.";

    [TextArea(10, 25)]
    public string techEraInstruction = @"[Tech Era - Cyber Energy and High-Dimensional Collection]
- Core Trade Logic: 'Efficiency and Scarcity'. Pragmatism looks towards energy density; romanticism looks towards historical relics.
- Hard Currency: Aggregated energy batteries (Polymer-cells), high-purity rare earths, and ancient physical keys that can bypass firewalls.
- Collector's Market (High Quoted Price): Genuine 'Stone Age Antiques' from tens of thousands of years ago (stone axes, dinosaur egg fossils, preserved paper books). These symbolize 'humanity's childhood' and carry high social prestige for future citizens.
- Extreme Devaluation: Ordinary mass-produced industrial goods are worthless in the age of matter synthesizers. Common wood, stone, and even fresh water have very low value unless their 'historical origin' is proven.
- Logical Taboos: Software installation discs that no longer have protocol support (e.g., Windows XP disc, as systems can't recognize them and drives are extinct), broken plastic trash.
- Culinary Culture: In the age of synthetic proteins, a 'naturally produced' cooked egg is considered a galaxy-class delicacy; prices should be exceptionally high.";

    public int minPrice = 0;
    public int maxPrice = 10000000;
}
