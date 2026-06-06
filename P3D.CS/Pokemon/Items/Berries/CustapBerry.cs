using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2061, "Custap")]
public class CustapBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public CustapBerry() : base(86400, "If held by a Pokémon, it gets to move first just once in a pinch.", "26.7cm", "Super Hard", 1, 5)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 40;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Ghost;
        Power = 100;
        JuiceColor = "red";
        JuiceGroup = 3;
    }

}
