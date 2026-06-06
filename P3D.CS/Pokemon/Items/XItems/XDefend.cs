using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(51, "X Defend")]
public class XDefend : XItem
{
    public override String Description { get; protected set; } = "Raises Defense.";
    public override bool CanBeUsedInBattle { get; } = true;

    public XDefend()
    {
        _textureRectangle = new Rectangle(24, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: battle stat boost (requires BattleScreen)
        return false;
    }
}
