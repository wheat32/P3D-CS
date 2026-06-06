using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(518, "Garchompite")]
public class Garchompite : MegaStone
{
    public Garchompite() : base("Garchompite", 445)
    {
        _textureRectangle = new Rectangle(24, 24, 24, 24);
    }

}
