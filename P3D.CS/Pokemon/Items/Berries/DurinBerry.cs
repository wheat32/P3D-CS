using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2033, "Durin")]
public class DurinBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public DurinBerry() : base(64800, "Pokéblock ingredient. Plant in loamy soil to grow Durin.", "28.0m", "Hard", 1, 2)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 30;
        Sour = 10;

        type = (int)Element.Types.Water;
        Power = 100;
        JuiceColor = "green";
        JuiceGroup = 1;
    }

}
