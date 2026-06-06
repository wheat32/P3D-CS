using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(190, "Heart Scale")]
public class HeartScale : Item
{
    public override String Description { get; protected set; } = "A pretty, heart-shaped scale that == extremely rare. It glows faintly with all of the colors of the rainbow.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public HeartScale()
    {
        _textureRectangle = new Rectangle(264, 216, 24, 24);
    }

}
