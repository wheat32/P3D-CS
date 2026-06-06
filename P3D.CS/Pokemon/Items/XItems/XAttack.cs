using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(50, "X Attack")]
public class XAttack : XItem
{
    public override String Description { get; protected set; } = "Raises Attack.";
    public override bool CanBeUsedInBattle { get; } = true;

    public XAttack()
    {
        _textureRectangle = new Rectangle(0, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: battle stat boost (requires BattleScreen)
        return false;
    }
}
