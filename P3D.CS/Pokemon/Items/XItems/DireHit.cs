using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(53, "Dire Hit")]
public class DireHit : XItem
{
    public override String Description { get; protected set; } = "Raises the critical-hit ratio.";
    public override bool CanBeUsedInBattle { get; } = true;

    public DireHit()
    {
        _textureRectangle = new Rectangle(144, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: battle stat boost (requires BattleScreen)
        return false;
    }
}
