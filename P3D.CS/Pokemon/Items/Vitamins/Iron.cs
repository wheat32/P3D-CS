using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Vitamins;

[Item(28, "Iron")]
public class Iron : VitaminItem
{
    public override String Description { get; protected set; } = "A nutritious drink for Pokémon. When consumed, it raises the base Defense stat of a single Pokémon.";
    public Iron()
    {
        _textureRectangle = new Rectangle(96, 24, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseVitamin(p.EVDefense, p) == true)
        {
            p.EVDefense += 10;
            p.ChangeFriendShip(Pokemon.FriendShipCauses.Vitamin);

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Defense.", [], false, false);
            PlayerStatistics.Track("[25]Vitamins used", 1);

            p.CalculateStats();
            RemoveItem();
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise~" + p.GetDisplayName() + "'s Defense.", [], false, false);

            return false;
        }
    }

}
