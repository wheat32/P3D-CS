using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.XItems;

[Item(55, "Guard Spec.")]
public class GuardSpec : XItem
{
    public override String Description { get; protected set; } = "Prevents stat reductions.";
    public override bool CanBeUsedInBattle { get; } = true;

    public GuardSpec()
    {
        _textureRectangle = new Rectangle(168, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            s = s.PreScreen;
        if (s.Identification != Screen.Identifications.BattleScreen)
            return false;
        BattleScreen bs = (BattleScreen)s;
        if (bs.FieldEffects.GuardSpec.Self > 0)
        {
            Screen.TextBox.Show("Guard Spec. is already active!", []);
            return false;
        }
        bs.FieldEffects.GuardSpec = (5, bs.FieldEffects.GuardSpec.Opponent);
        Screen.TextBox.Show("Guard Spec. protected " + bs.SelfPokemon!.GetDisplayName() + "!", []);
        RemoveItem();
        return true;
    }
}
