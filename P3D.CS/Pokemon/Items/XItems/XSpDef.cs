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
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleScreen bs = (BattleScreen)s;
        if (bs.Battle.RaiseStat(true, true, bs, "Special Defense", 1, String.Empty, "x sp def") == false)
            return false;
        RemoveItem();
        return true;
    }
}
