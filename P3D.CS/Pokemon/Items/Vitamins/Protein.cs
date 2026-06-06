using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Vitamins;

[Item(27, "Protein")]
public class Protein : VitaminItem
{
    public override String Description { get; protected set; } = "A nutritious drink for Pokémon. When consumed, it raises the base Attack stat of a single Pokémon.";
    public Protein()
    {
        _textureRectangle = new Rectangle(72, 24, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseVitamin(p.EVAttack, p) == true)
        {
            p.EVAttack += 10;
            p.ChangeFriendShip(Pokemon.FriendShipCauses.Vitamin);

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Attack.", [], false, false);
            PlayerStatistics.Track("[25]Vitamins used", 1);

            p.CalculateStats();
            RemoveItem();
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise~" + p.GetDisplayName() + "'s Attack.", [], false, false);

            return false;
        }
    }

}
