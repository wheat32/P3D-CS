using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(645, "Steel Gem")]
public class SteelGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of steel. When held, it strengthens the power of a Steel-type move one time.";
    public SteelGem() : base(Element.Types.Steel)
    {
    }

}
