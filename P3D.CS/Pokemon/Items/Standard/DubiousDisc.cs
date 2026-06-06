using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(185, "Dubious Disc")]
public class DubiousDisc : Item
{
    public override String Description { get; protected set; } = "A transparent device overflowing with dubious data. Its producer == unknown.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int FlingDamage { get; } = 50;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public DubiousDisc()
    {
        _textureRectangle = new Rectangle(0, 168, 24, 24);
    }

}
