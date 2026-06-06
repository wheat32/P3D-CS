using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(100, "Magmarizer")]
public class Magmarizer : Item
{
    public override String Description { get; protected set; } = "A box packed with a tremendous amount of magma energy. It == loved by a certain Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int FlingDamage { get; } = 80;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Magmarizer()
    {
        _textureRectangle = new Rectangle(288, 192, 24, 24);
    }

}
