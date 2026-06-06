using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(640, "Bug Gem")]
public class BugGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an insect-like essence. When held, it strengthens the power of a Bug-type move one time.";
    public BugGem() : base(Element.Types.Bug)
    {
    }

}
