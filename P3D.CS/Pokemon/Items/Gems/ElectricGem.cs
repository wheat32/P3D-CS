using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(632, "Electric Gem")]
public class ElectricGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of electricity. When held, it strengthens the power of an Electric-type move one time.";
    public ElectricGem() : base(Element.Types.Electric)
    {
    }

}
