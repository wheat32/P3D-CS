using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(13, "Paralyze Heal")]
public class ParalyzeHeal : MedicineItem
{
    public override String Description { get; protected set; } = "A spray-type medicine for paralysis. It can be used once to free a Pokémon that has been paralyzed.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public ParalyzeHeal()
    {
        _textureRectangle = new Rectangle(264, 0, 24, 24);
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
        return HealParalyze(PokeIndex);
    }

}
