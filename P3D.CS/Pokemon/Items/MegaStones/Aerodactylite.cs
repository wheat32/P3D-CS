using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(509, "Aerodactylite")]
public class Aerodactylite : MegaStone
{
    public Aerodactylite() : base("Aerodactyl", 142)
    {
        _textureRectangle = new Rectangle(48, 0, 24, 24);
    }

}
