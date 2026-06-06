using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Gems;

[Item(639, "Psychic Gem")]
public class PsychicGem : GemItem
{
    public override String Description { get; protected set; } = "A gem with an essence of the mind. When held, it strengthens the power of a Psychic-type move one time.";
    public PsychicGem() : base(Element.Types.Psychic)
    {
    }

}
