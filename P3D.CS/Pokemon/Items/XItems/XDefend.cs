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
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleScreen bs = (BattleScreen)s;
        if (bs.Battle.RaiseStat(true, true, bs, "Defense", 1, String.Empty, "x defend") == false)
            return false;
        RemoveItem();
        return true;
    }
}
