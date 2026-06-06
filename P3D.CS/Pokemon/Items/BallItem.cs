namespace P3D.Items.Balls;

public abstract class BallItem : Item
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeHeld { get; } = false;
    public override ItemTypes ItemType { get; } = ItemTypes.Pokeballs;
    public override int PokeDollarPrice { get; protected set; } = 1000;
}
