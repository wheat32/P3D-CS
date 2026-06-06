using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(642, "Ghost Gem")]
public class GhostGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with a spectral essence. When held, it strengthens the power of a Ghost-type move one time.";
    public GhostGem() : base(Element.Types.Ghost)
    {
    }

}
