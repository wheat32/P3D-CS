using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(636, "Poison Gem")]
public class PoisonGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of poison. When held, it strengthens the power of a Poison-type move one time.";
    public PoisonGem() : base(Element.Types.Poison)
    {
    }

}
