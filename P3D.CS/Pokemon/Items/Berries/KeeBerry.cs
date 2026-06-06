using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2065, "Kee")]
public class KeeBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public KeeBerry() : base(86400, "If held by a Pokémon, this Berry will increase the holder's Defense if it's hit with a physical move.", "5.0cm", "Very Soft", 1, 5)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 40;

        type = (int)Element.Types.Fairy;
        Power = 100;
        JuiceColor = "yellow";
        JuiceGroup = 3;
    }

}
