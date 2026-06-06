using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2056, "Apicot")]
public class ApicotBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public ApicotBerry() : base(86400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, its Sp. Def. stat will increase when it's in a pinch.", "7.6cm", "Very Hard", 1, 2)
    {

        Spicy = 10;
        Dry = 30;
        Sweet = 0;
        Bitter = 0;
        Sour = 30;

        type = (int)Element.Types.Ground;
        Power = 100;
        JuiceColor = "blue";
        JuiceGroup = 3;
    }

}
