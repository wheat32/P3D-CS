using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2024, "Grepa")]
public class GrepaBerry : Berry
{
    public override bool CanBeUsedInBattle { get; } = false;
    public GrepaBerry() : base(10800, "A Berry to be consumed by Pokémon. Using it on a Pokémon makes it more friendly but lowers its base Sp. Def.", "14.9cm", "Soft", 2, 6)
    {

        Spicy = 0;
        Dry = 10;
        Sweet = 10;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Flying;
        Power = 90;
        JuiceColor = "yellow";
        JuiceGroup = 3;
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

        if (p.EVSpDefense > 0)
        {
            int reduce = 10;
            if (p.EVSpDefense < reduce)
            {
                reduce = p.EVSpDefense;
            }

            p.ChangeFriendShip(Pokemon.FriendShipCauses.EVBerry);
            p.EVSpDefense -= reduce;
            p.CalculateStats();

            Screen.TextBox.Show("Raised friendship of~" + p.GetDisplayName() + "." + RemoveItem(), [], true, false);
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise the friendship~of " + p.GetDisplayName() + ".", [], true, false);
            return false;
        }
    }

}
