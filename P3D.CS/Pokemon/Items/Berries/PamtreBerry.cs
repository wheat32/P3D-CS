using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2031, "Pamtre")]
public class PamtreBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public PamtreBerry() : base(64800, "Pokéblock ingredient. Plant in loamy soil to grow Pamtre.", "24.4cm", "Very Soft", 1, 2)
    {

        Spicy = 0;
        Dry = 30;
        Sweet = 10;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Steel;
        Power = 90;
        JuiceColor = "purple";
        JuiceGroup = 1;
    }

}
