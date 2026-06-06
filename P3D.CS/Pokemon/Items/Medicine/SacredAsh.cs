using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(156, "Sacred Ash")]
public class SacredAsh : MedicineItem
{
    public override String Description { get; protected set; } = "It revives all fainted Pokémon. In doing so, it also fully restores their HP.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override String PluralName { get; } = "Sacred Ashes";
    public SacredAsh()
    {
        _textureRectangle = new Rectangle(24, 144, 24, 24);
    }

    public override void Use()
    {
        bool hasFainted = false;

        foreach (Pokemon p in Core.Player.Pokemons)
        {
            if (p.Status == Pokemon.StatusProblems.Fainted)
            {
                hasFainted = true;
                break;
            }
        }

        if (hasFainted == true)
        {
            foreach (Pokemon p in Core.Player.Pokemons)
            {
                if (p.Status == Pokemon.StatusProblems.Fainted)
                {
                    p.Status = Pokemon.StatusProblems.None;
                    p.HP = p.MaxHP;
                }
            }

            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show("Your team has been~fully healed." + RemoveItem(), []);
            PlayerStatistics.Track("[17]Medicine Items used", 1);
        }
        else
        {
            Screen.TextBox.Show("Cannot be used.", []);
        }
    }

}
