using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2060, "Micle")]
public class MicleBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public MicleBerry() : base(86400, "If held by a Pokémon, it raises the accuracy of a move just once in a pinch.", "4.1cm", "Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 40;
        Sweet = 10;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Rock;
        Power = 100;
        JuiceColor = "green";
        JuiceGroup = 3;
    }

}
