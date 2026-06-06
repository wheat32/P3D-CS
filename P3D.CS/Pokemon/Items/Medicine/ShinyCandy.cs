using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(501, "Shiny Candy")]
public class ShinyCandy : MedicineItem
{
    public override String Description { get; protected set; } = "This mysterious candy sparkles when unwrapped. It attracts all sorts of Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 4800;
    public override int MaxStack { get; } = 1;
    public override bool CanBeHeld { get; } = false;
    public override String PluralName { get; } = "Shiny Candies";
    public ShinyCandy()
    {
        _textureRectangle = new Rectangle(96, 240, 24, 24);
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
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (p.IsShiny == false)
        {
            p.IsShiny = true;

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(Localization.GetString("item_use_501", "The Pokémon sparkled.") + RemoveItem());
            PlayerStatistics.Track("[17]Medicine Items used", 1);

            return true;
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_OnPokemon_Single", "Cannot use [ITEMNAME]~on [POKEMONNAME].").Replace("[ITEMNAME]", Name).Replace("[POKEMONNAME]", p.GetDisplayName()), [], false, false);

            return false;
        }

    }

}
