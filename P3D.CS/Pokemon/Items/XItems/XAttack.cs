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
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleScreen bs = (BattleScreen)s;
        if (bs.Battle.RaiseStat(true, true, bs, "Attack", 1, String.Empty, "x attack") == false)
            return false;
        RemoveItem();
        return true;
    }
}
