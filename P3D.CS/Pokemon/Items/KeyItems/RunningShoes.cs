using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(78, "Running Shoes")]
public class RunningShoes : KeyItem
{
    public override String Description { get; protected set; } = "Special high-quality shoes. Instructions: Hold SHIFT to run!";
    public RunningShoes()
    {
        _textureRectangle = new Rectangle(288, 216, 24, 24);
    }

}
