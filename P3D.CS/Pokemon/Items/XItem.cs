namespace P3D.Items.XItems;

public abstract class XItem : Item
{
    public override bool CanBeUsed { get; } = false;
    public override bool BattleSelectPokemon { get; } = false;
    public override ItemTypes ItemType { get; } = ItemTypes.BattleItems;
}
