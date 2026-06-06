using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(124, "X Sp. Def")]
public class XSpDef : XItem
{
    public override String Description { get; protected set; } = "Raises Sp. Def.";
    public override bool CanBeUsedInBattle { get; } = true;

    public XSpDef()
    {
        _textureRectangle = new Rectangle(72, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: battle stat boost (requires BattleScreen)
        return false;
    }
}
