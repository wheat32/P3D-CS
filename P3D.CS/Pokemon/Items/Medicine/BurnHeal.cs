using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(10, "Burn Heal")]
public class BurnHeal : MedicineItem
{
    public override int PokeDollarPrice { get; protected set; } = 250;
    public override String Description { get; protected set; } = "A spray-type medicine for treating burns. It can be used once to heal a Pokemon suffering from a burn.";
    public BurnHeal()
    {
        _textureRectangle = new Rectangle(192, 0, 24, 24);
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
