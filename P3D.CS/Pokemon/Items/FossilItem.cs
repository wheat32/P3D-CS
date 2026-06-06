namespace P3D.Items;

public abstract class FossilItem : Item
{
    public override int PokeDollarPrice { get; protected set; } = 3500;
    public override int FlingDamage { get; } = 100;
    public override ItemTypes ItemType { get; } = ItemTypes.Standard;
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;

    protected FossilItem()
    {
        _textureSource = @"Items\Fossils";
    }
}
