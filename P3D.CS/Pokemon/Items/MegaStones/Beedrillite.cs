using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(537, "Beedrillite")]
public class Beedrillite : MegaStone
{
    public Beedrillite() : base("Beedrill", 15)
    {
        _textureRectangle = new Rectangle(48, 72, 24, 24);
    }

}
