using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(123, "X Sp. Atk")]
public class XSpAtk : XItem
{
    public override String Description { get; protected set; } = "Raises Sp. Atk.";
    public override bool CanBeUsedInBattle { get; } = true;

    public XSpAtk()
    {
        _textureRectangle = new Rectangle(48, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: battle stat boost (requires BattleScreen)
        return false;
    }
}
