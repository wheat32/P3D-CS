using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2010, "Figy")]
public class FigyBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public FigyBerry() : base(18000, "If held by a Pokémon, it restores the user's HP in a pinch, but it will cause confusion if the user hates the taste.", "10.0cm", "Soft", 2, 3)
    {

        Spicy = 15;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Bug;
        Power = 80;
        JuiceColor = "red";
        JuiceGroup = 3;
    }

}
