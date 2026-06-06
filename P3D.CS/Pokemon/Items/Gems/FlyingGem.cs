using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(638, "Flying Gem")]
public class FlyingGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of air. When held, it strengthens the power of a Flying-type move one time.";
    public FlyingGem() : base(Element.Types.Flying)
    {
    }

}
