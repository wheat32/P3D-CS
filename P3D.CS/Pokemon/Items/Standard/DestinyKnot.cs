using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(677, "Destiny Knot")]
public class DestinyKnot : Item
{
    public override String Description { get; protected set; } = "A long, thin, bright red string to be held by a Pokémon. If the holder becomes infatuated, the foe does too.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public DestinyKnot()
    {
        _textureRectangle = new Rectangle(432, 384, 24, 24);
    }

}
