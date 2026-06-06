using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plants;

[Item(253, "Honey")]
public class Honey : Item
{
    public override ItemTypes ItemType { get; } = ItemTypes.Standard;
    public override String Description { get; protected set; } = "Honey produced by a Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Honey()
    {
        _textureRectangle = new Rectangle(264, 240, 24, 24);
    }

}
