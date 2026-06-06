using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(1998, "Douse Drive")]
public class DouseDrive : Item
{
    public override String Description { get; protected set; } = "Makes Techno Blast a Water-type move if held by Genesect.";
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public DouseDrive()
    {
        _textureRectangle = new Rectangle(48, 312, 24, 24);
    }

}
