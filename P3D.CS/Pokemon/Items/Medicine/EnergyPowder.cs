using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(121, "Energy Powder")]
public class EnergyPowder : MedicineItem
{
    public override int PokeDollarPrice { get; protected set; } = 500;
    public override String Description { get; protected set; } = "A bitter medicine powder. When consumed, it restores 60 HP to an injured Pokémon.";
    public override bool IsHealingItem { get; } = true;
    public EnergyPowder()
    {
        _textureRectangle = new Rectangle(0, 120, 24, 24);
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
        bool success = HealPokemon(PokeIndex, 60);
        if (success)
        {
            Core.Player.Pokemons[PokeIndex].ChangeFriendShip(Pokemon.FriendShipCauses.EnergyPowder);
        }
        return success;
    }

}
