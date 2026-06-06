using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2009, "Sitrus")]
public class SitrusBerry : Berry
{
    public SitrusBerry() : base(21600, "A Berry to be consumed by Pokémon. If a Pokémon holds one, it can restore its own HP by a small amount during battle.", "9.5cm", "Very Hard", 2, 3)
    {

        Spicy = 0;
        Dry = 10;
        Sweet = 10;
        Bitter = 10;
        Sour = 10;

        type = (int)Element.Types.Psychic;
        Power = 80;
        JuiceColor = "yellow";
        JuiceGroup = 2;
    }

    public override void Use()
    {
        if (bool.Parse(GameModeManager.GetGameRuleValue("CanUseHealItems", "1")) == false)
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_HealingItems", "Cannot use healing items."), [], false, false);
            return;
        }
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
        return HealPokemon(PokeIndex, (int)(Math.Ceiling((double)(p.MaxHP / 4))));
    }

}
