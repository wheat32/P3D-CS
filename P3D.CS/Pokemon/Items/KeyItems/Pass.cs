using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(134, "Pass")]
public class Pass : KeyItem
{
    public override String Description { get; protected set; } = "A ticket required for riding the Magnet Train. It allows you to ride whenever && however much you'd like.";
    public Pass()
    {
        _textureRectangle = new Rectangle(312, 120, 24, 24);
    }

}
