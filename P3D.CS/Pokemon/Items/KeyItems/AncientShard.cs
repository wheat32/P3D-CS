using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(286, "Ancient Shard")]
public class AncientShard : KeyItem
{
    public override String Description { get; protected set; } = "A fragment of an ancient structure. It seems like it could fit somewhere.";
    public AncientShard()
    {
        _textureRectangle = new Rectangle(360, 288, 24, 24);
    }

}
