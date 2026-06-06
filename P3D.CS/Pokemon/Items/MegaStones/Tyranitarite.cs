using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(533, "Tyranitarite")]
public class Tyranitarite : MegaStone
{
    public Tyranitarite() : base("Tyranitar", 248)
    {
        _textureRectangle = new Rectangle(144, 48, 24, 24);
    }

}
