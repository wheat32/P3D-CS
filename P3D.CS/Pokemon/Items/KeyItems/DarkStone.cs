using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(652, "Dark Stone")]
public class DarkStone : KeyItem
{
    public override String Description { get; protected set; } = "Zekrom's body was destroyed && changed into this stone. It == said to be waiting for the emergence of a hero.";
    public DarkStone()
    {
        _textureRectangle = new Rectangle(240, 408, 24, 24);
    }

}
