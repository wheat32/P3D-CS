using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(644, "Dark Gem")]
public class DarkGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of darkness. When held, it strengthens the power of a Dark-type move one time.";
    public DarkGem() : base(Element.Types.Dark)
    {
    }

}
