using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2021, "Kelpsy")]
public class KelpsyBerry : Berry
{
    public override bool CanBeUsedInBattle { get; } = false;
    public KelpsyBerry() : base(10800, "A Berry to be consumed by Pokémon. Using it on a Pokémon makes it more friendly but lowers its base Attack.", "38.1cm", "Hard", 2, 6)
    {

        Spicy = 0;
        Dry = 10;
        Sweet = 0;
        Bitter = 10;
        Sour = 10;

        type = (int)Element.Types.Fighting;
        Power = 90;
        JuiceColor = "blue";
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

        if (p.EVAttack > 0)
        {
            int reduce = 10;
            if (p.EVAttack < reduce)
            {
                reduce = p.EVAttack;
            }

            p.ChangeFriendShip(Pokemon.FriendShipCauses.EVBerry);
            p.EVAttack -= reduce;
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
