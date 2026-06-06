using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(1996, "Burn Drive")]
public class BurnDrive : Item
{
    public override String Description { get; protected set; } = "Makes Techno Blast a Fire-type move if held by Genesect.";
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BurnDrive()
    {
        _textureRectangle = new Rectangle(0, 312, 24, 24);
    }

}
