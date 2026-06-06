using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Vitamins;

[Item(26, "HP Up")]
public class HPUp : VitaminItem
{
    public override String Description { get; protected set; } = "A nutritious drink for Pokémon. When consumed, it raises the base HP of a single Pokémon.";
    public HPUp()
    {
        _textureRectangle = new Rectangle(48, 24, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseVitamin(p.EVHP, p) == true)
        {
            p.EVHP += 10;
            p.ChangeFriendShip(Pokemon.FriendShipCauses.Vitamin);

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~HP.", [], false, false);
            PlayerStatistics.Track("[25]Vitamins used", 1);

            p.CalculateStats();
            RemoveItem();
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise~" + p.GetDisplayName() + "'s HP.", [], false, false);

            return false;
        }
    }

}
