using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(535, "Altarianite")]
public class Altarianite : MegaStone
{
    public Altarianite() : base("Altaria", 334)
    {
        _textureRectangle = new Rectangle(0, 72, 24, 24);
    }

}
