using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(657, "Catching Charm")]
public class CatchingCharm : KeyItem
{
    public override String Description { get; protected set; } = "Having one of these mysterious, unshakable charms makes it more likely youll get a critical catch.";
    public CatchingCharm()
    {
        _textureRectangle = new Rectangle(480, 408, 24, 24);
    }

}
