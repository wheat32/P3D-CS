using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(607, "Armor Fossil")]
public class ArmorFossil : FossilItem
{
    public override String Description { get; protected set; } = "A fossil from a prehistoric Pokémon that once lived on the land. It appears as though it's part of a head.";
    public ArmorFossil()
    {
        _textureRectangle = new Rectangle(24, 24, 24, 24);
    }

}
