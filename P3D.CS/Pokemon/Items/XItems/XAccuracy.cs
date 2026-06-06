using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(57, "X Accuracy")]
public class XAccuracy : XItem
{
    public override String Description { get; protected set; } = "Raises accuracy.";
    public override bool CanBeUsedInBattle { get; } = true;

    public XAccuracy()
    {
        _textureRectangle = new Rectangle(120, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: battle stat boost (requires BattleScreen)
        return false;
    }
}
