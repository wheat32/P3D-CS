using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(138, "Charcoal")]
public class Charcoal : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a combustible fuel that boosts the power of Fire-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 4900;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Charcoal()
    {
        _textureRectangle = new Rectangle(336, 120, 24, 24);
    }

}
