using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2030, "Spelon")]
public class SpelonBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public SpelonBerry() : base(64800, "Pokéblock ingredient." + Environment.NewLine + "Plant in loamy soil to grow Spelon.", "13.2cm", "Soft", 1, 2)
    {

        Spicy = 30;
        Dry = 10;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Dark;
        Power = 90;
        JuiceColor = "red";
        JuiceGroup = 1;
    }

}
