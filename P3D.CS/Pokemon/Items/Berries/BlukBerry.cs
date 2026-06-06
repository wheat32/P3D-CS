using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2016, "Bluk")]
public class BlukBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public BlukBerry() : base(3600, "Pokéblock ingredient. Plant in loamy soil to grow Bluk.", "10.8cm", "Soft", 3, 6)
    {

        Spicy = 0;
        Dry = 10;
        Sweet = 10;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Fire;
        Power = 90;
        JuiceColor = "purple";
        JuiceGroup = 1;
    }

}
