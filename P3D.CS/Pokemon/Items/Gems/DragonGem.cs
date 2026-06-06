using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(643, "Dragon Gem")]
public class DragonGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with a draconic essence. When held, it strengthens the power of a Dragon-type move one time.";
    public DragonGem() : base(Element.Types.Dragon)
    {
    }

}
