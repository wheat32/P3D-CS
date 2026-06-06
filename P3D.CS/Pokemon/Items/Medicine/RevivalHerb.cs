using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(124, "Revival Herb")]
public class RevivalHerb : Item
{
    public override bool IsHealingItem { get; } = true;
    public override String Description { get; protected set; } = "A terribly bitter medicinal herb. It revives a fainted Pokémon && fully restores its maximum HP.";
    public override int PokeDollarPrice { get; protected set; } = 2800;
    public override ItemTypes ItemType { get; } = ItemTypes.Medicine;
    public RevivalHerb()
    {
        _textureRectangle = new Rectangle(72, 120, 24, 24);
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
        Pokemon Pokemon = Core.Player.Pokemons[PokeIndex];

        if (Pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Pokemon.Status = P3D.Pokemon.StatusProblems.None;
            Pokemon.HP = (int)(Math.Floor((double)(Pokemon.MaxHP)));
            Pokemon.ChangeFriendShip(Pokemon.FriendShipCauses.RevivalHerb);

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(Localization.GetString("item_use_RevivalItem", "[POKEMONNAME]~is revitalized.").Replace("[POKEMONNAME]", Pokemon.GetDisplayName()), [], false, false);
            PlayerStatistics.Track("[17]Medicine Items used", 1);

            RemoveItem();

            return true;
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_OnPokemon_Single", "Cannot use [ITEMNAME]~on [POKEMONNAME].").Replace("[ITEMNAME]", Name).Replace("[POKEMONNAME]", Pokemon.GetDisplayName()), [], false, false);

            return false;
        }
    }

}
