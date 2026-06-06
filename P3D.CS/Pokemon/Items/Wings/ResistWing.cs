using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Wings;

[Item(256, "Resist Wing")]
public class ResistWing : WingItem
{
    public override String Description { get; protected set; } = "An item for use on a Pokémon. It slightly increases the base Defense stat of a single Pokémon.";
    public ResistWing()
    {
        _textureRectangle = new Rectangle(336, 240, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseWing(p.EVDefense, p) == true)
        {
            p.EVDefense += 1;

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Defense.", [], false, false);
            PlayerStatistics.Track("[254]Wings used", 1);

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
