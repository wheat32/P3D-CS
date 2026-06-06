using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(631, "Water Gem")]
public class WaterGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of water. When held, it strengthens the power of a Water-type move one time.";
    public WaterGem() : base(Element.Types.Water)
    {
    }

}
