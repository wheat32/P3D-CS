using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2019, "Pinap")]
public class PinapBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public PinapBerry() : base(3600, "Pokéblock ingredient. Plant in loamy soil to grow Pinap.", "8.0cm", "Hard", 3, 6)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Grass;
        Power = 90;
        JuiceColor = "yellow";
        JuiceGroup = 1;
    }

}
