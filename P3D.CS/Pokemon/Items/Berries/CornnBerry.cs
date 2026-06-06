using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2026, "Cornn")]
public class CornnBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public CornnBerry() : base(21600, "Pokéblock ingredient. Plant in loamy soil to grow Cornn.", "7.5cm", "Hard", 2, 4)
    {

        Spicy = 0;
        Dry = 20;
        Sweet = 10;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Bug;
        Power = 90;
        JuiceColor = "purple";
        JuiceGroup = 1;
    }

}
