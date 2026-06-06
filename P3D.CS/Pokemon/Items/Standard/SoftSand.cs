using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(76, "Soft Sand")]
public class SoftSand : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a loose, silky sand that boosts the power of Ground-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SoftSand()
    {
        _textureRectangle = new Rectangle(144, 72, 24, 24);
    }

}
