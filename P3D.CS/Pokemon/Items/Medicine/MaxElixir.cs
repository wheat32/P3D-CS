using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(21, "Max Elixir")]
public class MaxElixir : MedicineItem
{
    public override String Description { get; protected set; } = "This medicine can fully restore the PP of all of the moves that have been learned by a Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 4500;
    public MaxElixir()
    {
        _textureRectangle = new Rectangle(456, 0, 24, 24);
    }

    public override void Use()
    {
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
        bool missingPP = false;
        Pokemon Pokemon = Core.Player.Pokemons[PokeIndex];

        foreach (Attack Attack in Pokemon.Attacks)
        {
            if (Attack.currentPP < Attack.maxPP)
            {
                missingPP = true;
            }
            Attack.CurrentPP = Attack.maxPP;
        }

        if (missingPP == true)
        {
            String t = "Restored PP of~" + Pokemon.GetDisplayName() + "'s attacks.";
            t += RemoveItem();
            PlayerStatistics.Track("[17]Medicine Items used", 1);

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(t, [], true, true);
            return true;
        }
        else
        {
            Screen.TextBox.Show("The Pokémon's PP are~full already.", [], true, true);
            return false;
        }
    }

}
