using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(630, "Fire Gem")]
public class FireGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of fire. When held, it strengthens the power of a Fire-type move one time.";
    public FireGem() : base(Element.Types.Fire)
    {
    }

}
