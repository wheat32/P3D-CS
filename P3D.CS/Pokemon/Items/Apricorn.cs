namespace P3D.Items.Apricorns;

public abstract class Apricorn : Item
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public override int PokeDollarPrice { get; protected set; } = 100;
}
