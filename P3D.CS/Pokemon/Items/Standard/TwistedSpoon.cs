using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(96, "Twisted Spoon")]
public class TwistedSpoon : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a spoon imbued with telekinetic power that boosts Psychic-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public TwistedSpoon()
    {
        _textureRectangle = new Rectangle(480, 72, 24, 24);
    }

}
