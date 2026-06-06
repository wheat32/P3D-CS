using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2018, "Wepear")]
public class WepearBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public WepearBerry() : base(3600, "Pokéblock ingredient. Plant in loamy soil to grow Wepear.", "7.4cm", "Super Hard", 3, 6)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 10;
        Sour = 10;

        type = (int)Element.Types.Electric;
        Power = 90;
        JuiceColor = "green";
        JuiceGroup = 1;
    }

}
