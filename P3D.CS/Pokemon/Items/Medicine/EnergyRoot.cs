using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(122, "Energy Root")]
public class EnergyRoot : MedicineItem
{
    public override int PokeDollarPrice { get; protected set; } = 800;
    public override String Description { get; protected set; } = "An extremely bitter medicinal root. When consumed, it restores 120 HP to an injured Pokémon.";
    public override bool IsHealingItem { get; } = true;
    public EnergyRoot()
    {
        _textureRectangle = new Rectangle(24, 120, 24, 24);
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
        bool r = HealPokemon(PokeIndex, 120);
        if (r == true)
        {
            Core.Player.Pokemons[PokeIndex].ChangeFriendShip(Pokemon.FriendShipCauses.EnergyRoot);
        }
        return r;
    }

}
