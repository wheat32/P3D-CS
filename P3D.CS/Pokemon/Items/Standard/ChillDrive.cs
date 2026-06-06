using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(1997, "Chill Drive")]
public class ChillDrive : Item
{
    public override String Description { get; protected set; } = "Makes Techno Blast an Ice-type move if held by Genesect.";
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ChillDrive()
    {
        _textureRectangle = new Rectangle(24, 312, 24, 24);
    }

}
