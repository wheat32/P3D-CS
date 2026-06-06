using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Vitamins;

[Item(25, "Zinc")]
public class Zinc : VitaminItem
{
    public override String Description { get; protected set; } = "A nutritious drink for Pokémon. When consumed, it raises the base Sp. Def. stat of a single Pokémon.";
    public Zinc()
    {
        _textureRectangle = new Rectangle(168, 192, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseVitamin(p.EVSpDefense, p) == true)
        {
            p.EVSpDefense += 10;
            p.ChangeFriendShip(Pokemon.FriendShipCauses.Vitamin);

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Special Defense.", [], false, false);
            PlayerStatistics.Track("[25]Vitamins used", 1);

            p.CalculateStats();
            RemoveItem();
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise~" + p.GetDisplayName() + "'s Special~Defense.", [], false, false);

            return false;
        }
    }

}
