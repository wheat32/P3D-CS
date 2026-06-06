using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(123, "Heal Powder")]
public class HealPowder : MedicineItem
{
    public override String Description { get; protected set; } = "A very bitter medicine powder. When consumed, it heals all of a Pokémon's status conditions.";
    public override int PokeDollarPrice { get; protected set; } = 450;
    public HealPowder()
    {
        _textureRectangle = new Rectangle(48, 120, 24, 24);
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
        Pokemon Pokemon = Core.Player.Pokemons[PokeIndex];

        if (Pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Screen.TextBox.reDelay = 0.0F;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_IsFainted", "[POKEMONNAME]~is fainted!").Replace("[POKEMONNAME]", Pokemon.GetDisplayName()), []);

            return false;
        }
        else
        {
            if (Pokemon.Status != P3D.Pokemon.StatusProblems.None)
            {
                Pokemon.Status = P3D.Pokemon.StatusProblems.None;
                Pokemon.ChangeFriendShip(Pokemon.FriendShipCauses.HealPowder);

                Core.Player.Inventory.RemoveItem(ID.ToString(), 1);

                Screen.TextBox.reDelay = 0.0F;

                String t = Pokemon.GetDisplayName() + "~gets healed up!";
                t += RemoveItem();

                SoundManager.PlaySound("Use_Item", false);
                Screen.TextBox.Show(t, []);
                PlayerStatistics.Track("[17]Medicine Items used", 1);

                return true;
            }
            else
            {
                Screen.TextBox.reDelay = 0.0F;
                Screen.TextBox.Show(Pokemon.GetDisplayName() + "~is fully healed!", []);

                return false;
            }
        }
    }

}
