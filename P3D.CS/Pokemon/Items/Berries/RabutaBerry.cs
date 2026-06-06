using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2028, "Rabuta")]
public class RabutaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public RabutaBerry() : base(21600, "Pokéblock ingredient. Plant in loamy soil to grow Rabuta.", "22.6cm", "Soft", 2, 4)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 20;
        Sour = 10;

        type = (int)Element.Types.Ghost;
        Power = 90;
        JuiceColor = "green";
        JuiceGroup = 1;
    }

}
