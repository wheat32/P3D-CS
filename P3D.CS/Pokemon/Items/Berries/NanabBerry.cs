using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2017, "Nanab")]
public class NanabBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public NanabBerry() : base(3600, "Pokéblock ingredient. Plant in loamy soil to grow Nanab.", "7.7cm", "Very Hard", 2, 3)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 10;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Water;
        Power = 90;
        JuiceColor = "pink";
        JuiceGroup = 1;
    }

}
