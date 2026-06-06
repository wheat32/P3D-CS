using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(646, "Normal Gem")]
public class NormalGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an ordinary essence. When held, it strengthens the power of a Normal-type move one time.";
    public NormalGem() : base(Element.Types.Normal)
    {
    }

}
