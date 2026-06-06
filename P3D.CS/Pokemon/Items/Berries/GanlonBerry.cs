using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2053, "Ganlon")]
public class GanlonBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public GanlonBerry() : base(86400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, its Defense stat will increase when it's in a pinch.", "3.3cm", "Very Hard", 1, 2)
    {

        Spicy = 0;
        Dry = 30;
        Sweet = 10;
        Bitter = 30;
        Sour = 0;

        type = (int)Element.Types.Ice;
        Power = 100;
        JuiceColor = "purple";
        JuiceGroup = 3;
    }

}
