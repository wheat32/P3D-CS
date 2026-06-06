namespace P3D.Items.KeyItems;

public abstract class KeyItem : Item
{
    public override ItemTypes ItemType { get; } = ItemTypes.KeyItems;
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeTraded { get; protected set; } = false;
    public override bool CanBeTossed { get; protected set; } = false;
    public override bool CanBeHeld { get; } = false;
    public override int PokeDollarPrice { get; protected set; } = 9800;
}
