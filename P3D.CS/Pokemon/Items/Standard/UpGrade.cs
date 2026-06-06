using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(172, "Up-Grade")]
public class UpGrade : Item
{
    public override String Description { get; protected set; } = "A transparent device somehow filled with all sorts of data. It was produced by Silph Co.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public UpGrade()
    {
        _textureRectangle = new Rectangle(336, 144, 24, 24);
    }

}
