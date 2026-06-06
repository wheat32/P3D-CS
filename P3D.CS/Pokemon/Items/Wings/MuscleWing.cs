using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Wings;

[Item(255, "Muscle Wing")]
public class MuscleWing : WingItem
{
    public override String Description { get; protected set; } = "An item for use on a Pokémon. It slightly increases the base Attack stat of a single Pokémon.";
    public MuscleWing()
    {
        _textureRectangle = new Rectangle(312, 240, 24, 24);
    }

    public override bool UseOnPokemon(int PokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[PokeIndex];

        if (CanUseWing(p.EVAttack, p) == true)
        {
            p.EVAttack += 1;

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Raised " + p.GetDisplayName() + "'s~Attack.", [], false, false);
            PlayerStatistics.Track("[254]Wings used", 1);

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
