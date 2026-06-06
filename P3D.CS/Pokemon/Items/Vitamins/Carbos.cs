using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Vitamins;

[Item(29, "Carbos")]
public class Carbos : VitaminItem
{
    public override String Description { get; protected set; } = "A nutritious drink for Pokémon. When consumed, it raises the base Speed stat of a single Pokémon.";
    public override String PluralName { get; } = "Carboses";
    public Carbos()
    {
        _textureRectangle = new Rectangle(120, 24, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseVitamin(p.EVSpeed, p) == true)
        {
            p.EVSpeed += 10;
            p.ChangeFriendShip(Pokemon.FriendShipCauses.Vitamin);

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Speed.", [], false, false);
            PlayerStatistics.Track("[25]Vitamins used", 1);

            p.CalculateStats();
            RemoveItem();
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise~" + p.GetDisplayName() + "'s Speed.", [], false, false);

            return false;
        }
    }

}
