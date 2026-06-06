using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(242, "Shiny Charm")]
public class ShinyCharm : KeyItem
{
    public override String Description { get; protected set; } = "A shiny charm said to increase the chance of encountering Shiny Pokémon.";
    public ShinyCharm()
    {
        _textureRectangle = new Rectangle(120, 264, 24, 24);
    }

}
