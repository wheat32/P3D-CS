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
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleScreen bs = (BattleScreen)s;
        if (bs.Battle.RaiseStat(true, true, bs, "Special Attack", 1, String.Empty, "x sp atk") == false)
            return false;
        RemoveItem();
        return true;
    }
}
