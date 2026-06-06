using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(637, "Ground Gem")]
public class GroundGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of land. When held, it strengthens the power of a Ground-type move one time.";
    public GroundGem() : base(Element.Types.Ground)
    {
    }

}
