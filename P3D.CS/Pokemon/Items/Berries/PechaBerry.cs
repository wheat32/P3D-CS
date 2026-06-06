using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2002, "Pecha")]
public class PechaBerry : Berry
{
    public PechaBerry() : base(10800, "A Berry to be consumed by Pokémon. If a Pokémon holds one, it can recover from poisoning on its own in battle.", "4.0cm", "Very Soft", 2, 3)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 10;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Electric;
        Power = 80;
        JuiceColor = "pink";
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
        return CurePoison(PokeIndex);
    }

}
