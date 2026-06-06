using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2027, "Magost")]
public class MagostBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public MagostBerry() : base(21600, "Pokéblock ingredient. Plant in loamy soil to grow Magost.", "14.0cm", "Hard", 2, 4)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 20;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Rock;
        Power = 90;
        JuiceColor = "pink";
        JuiceGroup = 1;
    }

}
