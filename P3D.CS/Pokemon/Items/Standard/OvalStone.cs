using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(179, "Oval Stone")]
public class OvalStone : Item
{
    public override String Description { get; protected set; } = "A peculiar stone that makes certain species of Pokémon evolve. It == shaped like an egg.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int FlingDamage { get; } = 80;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public OvalStone()
    {
        _textureRectangle = new Rectangle(192, 216, 24, 24);
    }

}
