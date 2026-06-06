namespace P3D.Items;

public abstract class MegaStone : Item
{
    public int MegaPokemonNumber { get; }

    public override String Description { get; protected set; }
    public override bool CanBeTossed { get; protected set; } = false;
    public override bool CanBeTraded { get; protected set; } = false;
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;

    protected MegaStone(String megaPokemonName, int megaPokemonNumber)
    {
        Description = $"One variety of the mysterious Mega Stones. Have {megaPokemonName} hold it, and this stone will enable it to Mega Evolve during battle.";
        _textureSource = @"Items\MegaStones";
        MegaPokemonNumber = megaPokemonNumber;
    }
}
