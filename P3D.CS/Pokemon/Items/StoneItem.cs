using P3D;

namespace P3D.Items;

public abstract class StoneItem : Item
{
    public override bool CanBeUsedInBattle { get; } = false;
    public override int PokeDollarPrice { get; protected set; } = 2100;

    public override void Use()
    {
        if (Core.Player.Pokemons.Count > 0)
        {
            PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, this, UseOnPokemon,
                Localization.GetString("global_use", "Use") + " " + OneLineName(), true)
            {
                Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
                CanExit = true,
            };
            selScreen.SelectedObject += UseItemhandler;
            Core.SetScreen(selScreen);
            ((PartyScreen)Core.CurrentScreen).EvolutionItemID = ID.ToString();
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_NoPokemon", "You don't have any Pokémon."), [], false, false);
        }
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        return UseStone(pokeIndex);
    }

    public bool UseStone(int pokeIndex)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Pokemon p = Core.Player.Pokemons[pokeIndex];
        if (p.IsEgg == false && p.CanEvolve(EvolutionCondition.EvolutionTrigger.UseItem, ID.ToString()) == true)
        {
            RemoveItem();
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen,
                new EvolutionScreen(Core.CurrentScreen, [pokeIndex], ID.ToString(), EvolutionCondition.EvolutionTrigger.UseItem),
                Microsoft.Xna.Framework.Color.Black, false));
            PlayerStatistics.Track("[22]Evolution stones used", 1);
            return true;
        }
        Screen.TextBox.Show(
            Localization.GetString("item_cannot_use_OnPokemon_Single", "Cannot use [ITEMNAME]~on [POKEMONNAME].")
                .Replace("[ITEMNAME]", Name)
                .Replace("[POKEMONNAME]", p.GetDisplayName()), [], false, false);
        return false;
    }
}
