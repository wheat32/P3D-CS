using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(120, "Electirizer")]
public class Electirizer : Item
{
    public override String Description { get; protected set; } = "A box packed with a tremendous amount of electric energy. It == loved by a certain Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int FlingDamage { get; } = 80;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Electirizer()
    {
        _textureRectangle = new Rectangle(312, 192, 24, 24);
    }

}
