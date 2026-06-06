using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2029, "Nomel")]
public class NomelBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public NomelBerry() : base(21600, "Pokéblock ingredient. Plant in loamy soil to grow Nomel.", "28.5cm", "Soft", 2, 4)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 20;

        type = (int)Element.Types.Dragon;
        Power = 90;
        JuiceColor = "yellow";
        JuiceGroup = 1;
    }

}
