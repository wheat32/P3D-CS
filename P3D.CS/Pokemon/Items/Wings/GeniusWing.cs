using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Wings;

[Item(257, "Genius Wing")]
public class GeniusWing : WingItem
{
    public override String Description { get; protected set; } = "An item for use on a Pokémon. It slightly increases the base Sp. Atk. stat of a single Pokémon.";
    public GeniusWing()
    {
        _textureRectangle = new Rectangle(360, 240, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseWing(p.EVSpAttack, p) == true)
        {
            p.EVSpAttack += 1;

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Special Attack.", [], false, false);
            PlayerStatistics.Track("[254]Wings used", 1);

            p.CalculateStats();
            RemoveItem();
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise~" + p.GetDisplayName() + "'s Special Attack.", [], false, false);

            return false;
        }
    }

}
