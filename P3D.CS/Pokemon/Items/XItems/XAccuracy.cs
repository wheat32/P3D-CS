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
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleScreen bs = (BattleScreen)s;
        if (bs.Battle.RaiseStat(true, true, bs, "Accuracy", 1, String.Empty, "x accuracy") == false)
            return false;
        RemoveItem();
        return true;
    }
}
