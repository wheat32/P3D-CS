using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2025, "Tamato")]
public class TamatoBerry : Berry
{
    public override bool CanBeUsedInBattle { get; } = false;
    public TamatoBerry() : base(21600, "A Berry to be consumed by Pokémon. Using it on a Pokémon makes it more friendly but lowers its base Speed.", "20.0m", "Soft", 2, 4)
    {

        Spicy = 20;
        Dry = 10;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Psychic;
        Power = 90;
        JuiceColor = "red";
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

        if (p.EVSpeed > 0)
        {
            int reduce = 10;
            if (p.EVSpeed < reduce)
            {
                reduce = p.EVSpeed;
            }

            p.ChangeFriendShip(Pokemon.FriendShipCauses.EVBerry);
            p.EVSpeed -= reduce;
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
