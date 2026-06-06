using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(113, "Spell Tag")]
public class SpellTag : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a sinister, eerie tag that boosts the power of Ghost-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SpellTag()
    {
        _textureRectangle = new Rectangle(336, 96, 24, 24);
    }

}
