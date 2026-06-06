using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Wings;

[Item(254, "Health Wing")]
public class HealthWing : WingItem
{
    public override String Description { get; protected set; } = "An item for use on a Pokémon. It slightly increases the base HP stat of a single Pokémon.";
    public HealthWing()
    {
        _textureRectangle = new Rectangle(288, 240, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseWing(p.EVHP, p) == true)
        {
            p.EVHP += 1;

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~HP.", [], false, false);
            PlayerStatistics.Track("[254]Wings used", 1);

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
