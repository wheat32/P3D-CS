using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2023, "Hondew")]
public class HondewBerry : Berry
{
    public override bool CanBeUsedInBattle { get; } = false;
    public HondewBerry() : base(10800, "A Berry to be consumed by Pokémon. Using it on a Pokémon makes it more friendly but lowers its base Sp. Atk.", "16.2cm", "Hard", 2, 6)
    {

        Spicy = 10;
        Dry = 10;
        Sweet = 0;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Ground;
        Power = 90;
        JuiceColor = "green";
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

        if (p.EVSpAttack > 0)
        {
            int reduce = 10;
            if (p.EVSpAttack < reduce)
            {
                reduce = p.EVSpAttack;
            }

            p.ChangeFriendShip(Pokemon.FriendShipCauses.EVBerry);
            p.EVSpAttack -= reduce;
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
