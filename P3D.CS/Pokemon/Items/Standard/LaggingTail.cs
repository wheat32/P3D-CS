using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(142, "Lagging Tail")]
public class LaggingTail : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == tremendously heavy && makes the holder move slower than usual.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LaggingTail()
    {
        _textureRectangle = new Rectangle(432, 192, 24, 24);
    }

}
