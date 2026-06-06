using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(1999, "Shock Drive")]
public class ShockDrive : Item
{
    public override String Description { get; protected set; } = "Makes Techno Blast an Electric-type move if held by Genesect.";
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ShockDrive()
    {
        _textureRectangle = new Rectangle(72, 312, 24, 24);
    }

}
