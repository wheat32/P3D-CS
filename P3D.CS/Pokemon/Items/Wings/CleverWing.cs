using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Wings;

[Item(258, "Clever Wing")]
public class CleverWing : WingItem
{
    public override String Description { get; protected set; } = "An item for use on a Pokémon. It slightly increases the base Sp. Def. stat of a single Pokémon.";
    public CleverWing()
    {
        _textureRectangle = new Rectangle(384, 240, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseWing(p.EVSpDefense, p) == true)
        {
            p.EVSpDefense += 1;

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Special Defense.", [], false, false);
            PlayerStatistics.Track("[254]Wings used", 1);

            p.CalculateStats();
            RemoveItem();
            return true;
        }
        else
        {
            Screen.TextBox.Show("Cannot raise~" + p.GetDisplayName() + "'s Special Defense.", [], false, false);

            return false;
        }
    }

}
