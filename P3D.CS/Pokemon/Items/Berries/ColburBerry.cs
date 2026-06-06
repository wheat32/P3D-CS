using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2049, "Colbur")]
public class ColburBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public ColburBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Dark-type attack.", "3.8cm", "Super Hard", 1, 5)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 10;
        Sour = 20;

        type = (int)Element.Types.Dark;
        Power = 80;
        JuiceColor = "purple";
        JuiceGroup = 2;
    }

}
