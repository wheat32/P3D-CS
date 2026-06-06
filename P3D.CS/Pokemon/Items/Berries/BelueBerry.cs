using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2034, "Belue")]
public class BelueBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public BelueBerry() : base(64800, "Pokéblock ingredient. Plant in loamy soil to grow Belue.", "11.8cm", "Very Soft", 1, 2)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 30;

        type = (int)Element.Types.Electric;
        Power = 100;
        JuiceColor = "purple";
        JuiceGroup = 1;
    }

}
