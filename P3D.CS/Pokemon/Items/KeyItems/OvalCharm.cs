using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(241, "Oval Charm")]
public class OvalCharm : KeyItem
{
    public override String Description { get; protected set; } = "An oval charm said to increase the chance of Pokémon Eggs being found at the Day Care.";
    public OvalCharm()
    {
        _textureRectangle = new Rectangle(96, 264, 24, 24);
    }

}
