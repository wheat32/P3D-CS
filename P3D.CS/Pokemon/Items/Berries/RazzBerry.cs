using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2015, "Razz")]
public class RazzBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public RazzBerry() : base(3600, "Pokéblock ingredient. Plant in loamy soil to grow Razz.", "12.0cm", "Very Hard", 2, 3)
    {

        Spicy = 10;
        Dry = 10;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Steel;
        Power = 80;
        JuiceColor = "red";
        JuiceGroup = 1;
    }

}
