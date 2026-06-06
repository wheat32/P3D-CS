using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2003, "Rawst")]
public class RawstBerry : Berry
{
    public RawstBerry() : base(10800, "A Berry to be consumed by Pokémon. If a Pokémon holds one, it can recover from a burn on its own in battle.", "3.2cm", "Hard", 2, 3)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Grass;
        Power = 80;
        JuiceColor = "green";
        JuiceGroup = 1;
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
        return HealBurn(PokeIndex);
    }

}
