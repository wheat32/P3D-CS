using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(16, "Hyper Potion")]
public class HyperPotion : MedicineItem
{
    public override bool IsHealingItem { get; } = true;
    public override String Description { get; protected set; } = "A spray-type medicine for treating wounds. It can be used to restore 120 HP to an injured Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 1500;
    public HyperPotion()
    {
        _textureRectangle = new Rectangle(336, 0, 24, 24);
    }

    public override void Use()
    {
        if (bool.Parse(GameModeManager.GetGameRuleValue("CanUseHealItems", "1")) == false)
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_HealingItems", "Cannot use healing items."), [], false, false);
            return;
        }
        if (Core.Player.Pokemons.Count > 0)
        {
            PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, this, UseOnPokemon, Localization.GetString("global_use", "Use") + " " + OneLineName(), true) {
            Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
            CanExit = true,
        };
            selScreen.SelectedObject += UseItemhandler;

            Core.SetScreen(selScreen);
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_NoPokemon", "You don't have any Pokémon."), [], false, false);
        }
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        return HealPokemon(PokeIndex, 120);
    }

}
