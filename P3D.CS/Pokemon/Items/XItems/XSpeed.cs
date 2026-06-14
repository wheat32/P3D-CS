using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(52, "X Speed")]
public class XSpeed : XItem
{
    public override String Description { get; protected set; } = "Raises Speed.";
    public override bool CanBeUsedInBattle { get; } = true;

    public XSpeed()
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
        if (bs.Battle.RaiseStat(true, true, bs, "Speed", 1, String.Empty, "x speed") == false)
            return false;
        RemoveItem();
        return true;
    }
}
