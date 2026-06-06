using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2059, "Enigma")]
public class EnigmaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public EnigmaBerry() : base(86400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, being hit by a supereffective attack will restore its HP.", "15.5cm", "Hard", 1, 2)
    {

        Spicy = 40;
        Dry = 10;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Bug;
        Power = 100;
        JuiceColor = "purple";
        JuiceGroup = 3;
    }

}
