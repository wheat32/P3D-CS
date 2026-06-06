using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2007, "Persim")]
public class PersimBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public PersimBerry() : base(14400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, it can recover from confusion on its own in battle.", "4.7cm", "Hard", 2, 3)
    {

        Spicy = 10;
        Dry = 10;
        Sweet = 10;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Ground;
        Power = 80;
        JuiceColor = "pink";
        JuiceGroup = 2;
    }

}
