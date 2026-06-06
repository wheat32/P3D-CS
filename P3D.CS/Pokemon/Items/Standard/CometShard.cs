using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(149, "Comet Shard")]
public class CometShard : Item
{
    public override String Description { get; protected set; } = "A shard which fell to the ground when a comet approached. A maniac will buy it for a high price.";
    public override int PokeDollarPrice { get; protected set; } = 120000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public CometShard()
    {
        _textureRectangle = new Rectangle(24, 216, 24, 24);
    }

}
