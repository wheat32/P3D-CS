using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(647, "Fairy Gem")]
public class FairyGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of the fey. When held, it strengthens the power of a Fairy-type move one time.";
    public FairyGem() : base(Element.Types.Fairy)
    {
    }

}
