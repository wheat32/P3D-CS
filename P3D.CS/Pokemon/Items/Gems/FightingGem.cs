using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(635, "Fighting Gem")]
public class FightingGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of combat. When held, it strengthens the power of a Fighting-type move one time.";
    public FightingGem() : base(Element.Types.Fighting)
    {
    }

}
