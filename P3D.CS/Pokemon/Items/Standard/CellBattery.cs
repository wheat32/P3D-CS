using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(595, "Cell Battery")]
public class CellBattery : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It boosts Attack if hit with an Electric-type attack. It can only be used once.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Cell Batteries";
    public CellBattery()
    {
        _textureRectangle = new Rectangle(192, 312, 24, 24);
    }

}
