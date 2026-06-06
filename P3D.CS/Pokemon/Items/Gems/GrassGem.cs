using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(633, "Grass Gem")]
public class GrassGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of nature. When held, it strengthens the power of a Grass-type move one time.";
    public GrassGem() : base(Element.Types.Grass)
    {
    }

}
