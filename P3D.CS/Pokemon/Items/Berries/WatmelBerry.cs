using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2032, "Watmel")]
public class WatmelBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public WatmelBerry() : base(64800, "Pokéblock ingredient. Plant in loamy soil to grow Watmel.", "25.0cm", "Soft", 1, 2)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 30;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Fire;
        Power = 100;
        JuiceColor = "pink";
        JuiceGroup = 1;

    }

}
