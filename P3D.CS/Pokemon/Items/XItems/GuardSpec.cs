using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(55, "Guard Spec.")]
public class GuardSpec : XItem
{
    public override String Description { get; protected set; } = "Prevents stat reductions.";
    public override bool CanBeUsedInBattle { get; } = true;

    public GuardSpec()
    {
        _textureRectangle = new Rectangle(168, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: battle guard logic (requires BattleScreen)
        return false;
    }
}
