using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2011, "Wiki")]
public class WikiBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public WikiBerry() : base(21600, "If held by a Pokémon, it restores the user's HP in a pinch, but it will cause confusion if the user hates the taste.", "11.5cm", "Hard", 2, 3)
    {

        Spicy = 0;
        Dry = 15;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Rock;
        Power = 80;
        JuiceColor = "purple";
        JuiceGroup = 3;
    }

}
