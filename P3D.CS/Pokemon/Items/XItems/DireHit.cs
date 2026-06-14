using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(53, "Dire Hit")]
public class DireHit : XItem
{
    public override String Description { get; protected set; } = "Raises the critical-hit ratio.";
    public override bool CanBeUsedInBattle { get; } = true;

    public DireHit()
    {
        _textureRectangle = new Rectangle(144, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleScreen bs = (BattleScreen)s;
        if (bs.FieldEffects.FocusEnergy.Self > 0)
        {
            Screen.TextBox.Show(bs.SelfPokemon!.GetDisplayName() + " is already pumped up!", []);
            return false;
        }
        bs.FieldEffects.FocusEnergy = (1, bs.FieldEffects.FocusEnergy.Opponent);
        Screen.TextBox.Show(bs.SelfPokemon!.GetDisplayName() + " is getting pumped!", []);
        RemoveItem();
        return true;
    }
}
