using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(577, "Ability Capsule")]
public class AbilityCapsule : Item
{
    public override String Description { get; protected set; } = "A capsule that allows a Pokémon to switch between its two Abilities.";
    public override int PokeDollarPrice { get; protected set; } = 0;
    public override bool CanBeUsedInBattle { get; } = false;

    public AbilityCapsule()
    {
        _textureRectangle = new Rectangle(120, 264, 24, 24);
    }

    public override void Use()
    {
        if (Core.Player.Pokemons.Count > 0)
        {
            PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, this, UseOnPokemon,
                Localization.GetString("global_use", "Use") + " " + OneLineName(), true)
            {
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

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 3: ability slot swap logic (requires Ability backing field access)
        return false;
    }
}
