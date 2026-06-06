using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2054, "Salac")]
public class SalacBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public SalacBerry() : base(86400, "A Berry to be held by Pokémon. If a Pokémon holds one, its Speed stat will increase when it's in a pinch. ", "9.4cm", "Very Hard", 1, 2)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 30;
        Bitter = 10;
        Sour = 30;

        type = (int)Element.Types.Fighting;
        Power = 100;
        JuiceColor = "green";
        JuiceGroup = 3;
    }

}
