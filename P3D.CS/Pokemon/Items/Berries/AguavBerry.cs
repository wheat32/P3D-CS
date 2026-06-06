using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2013, "Aguav")]
public class AguavBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public AguavBerry() : base(21600, "If held by a Pokémon, it restores the user's HP in a pinch, but it will cause confusion if the user hates the taste.", "6.4cm", "Super Hard", 2, 3)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Dragon;
        Power = 80;
        JuiceColor = "green";
        JuiceGroup = 3;
    }

}
