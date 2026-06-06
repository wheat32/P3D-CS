using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2022, "Qualot")]
public class QualotBerry : Berry
{
    public override bool CanBeUsedInBattle { get; } = false;
    public QualotBerry() : base(10800, "A Berry to be consumed by Pokémon. Using it on a Pokémon makes it more friendly but lowers its base Defense.", "11.0cm", "Hard", 2, 6)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 10;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Poison;
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

        if (p.EVDefense > 0)
        {
            int reduce = 10;
            if (p.EVDefense < reduce)
            {
                reduce = p.EVDefense;
            }

            p.ChangeFriendShip(Pokemon.FriendShipCauses.EVBerry);
            p.EVDefense -= reduce;
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
