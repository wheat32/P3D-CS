using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(658, "Exp. All")]
public class ExpAll : KeyItem
{
    public override String Description { get; protected set; } = "Turning on this special device will allow all the Pokémon on your team to receive Exp. Points from battles.";
    public override int PokeDollarPrice { get; protected set; } = 3000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = true;
    public ExpAll()
    {
        _textureRectangle = new Rectangle(216, 48, 24, 24);
    }

    public override void Use()
    {
        Core.Player.EnableExpAll = Core.Player.EnableExpAll == false;
        if (Core.Player.EnableExpAll == false)
        {
            Screen.TextBox.Show(Localization.GetString("item_use_658_disable", "The Exp. All was turned off."));
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("item_use_658_enable", "The Exp. All was turned on."));
        }

    }

}
