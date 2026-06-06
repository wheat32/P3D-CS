using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(634, "Ice Gem")]
public class IceGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of ice. When held, it strengthens the power of an Ice-type move one time.";
    public IceGem() : base(Element.Types.Ice)
    {
    }

}
