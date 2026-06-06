using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(534, "Venusaurite")]
public class Venusaurite : MegaStone
{
    public Venusaurite() : base("Venusaur", 3)
    {
        _textureRectangle = new Rectangle(168, 48, 24, 24);
    }

}
