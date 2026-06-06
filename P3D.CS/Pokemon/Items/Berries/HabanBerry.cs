using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2048, "Haban")]
public class HabanBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public HabanBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Dragon-type attack.", "2.3cm", "Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 10;
        Bitter = 20;
        Sour = 0;

        type = (int)Element.Types.Dragon;
        Power = 80;
        JuiceColor = "red";
        JuiceGroup = 2;
    }

}
