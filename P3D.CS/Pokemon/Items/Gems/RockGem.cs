using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(641, "Rock Gem")]
public class RockGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of rock. When held, it strengthens the power of a Rock-type move one time.";
    public RockGem() : base(Element.Types.Rock)
    {
    }

}
