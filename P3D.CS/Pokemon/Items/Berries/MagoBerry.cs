using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2012, "Mago")]
public class MagoBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public MagoBerry() : base(21600, "If held by a Pokémon, it restores the user's HP in a pinch, but it will cause confusion if the user hates the taste.", "12.6cm", "Hard", 2, 3)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 15;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Ghost;
        Power = 80;
        JuiceColor = "pink";
        JuiceGroup = 3;
    }

}
