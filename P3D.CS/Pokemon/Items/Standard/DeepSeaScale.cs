using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(162, "DeepSeaScale")]
public class DeepSeaScale : Item
{
    public override String Description { get; protected set; } = "An item to be held by Clamperl. This scale shines with a faint pink && raises the holder's Sp. Def. stat.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public DeepSeaScale()
    {
        _textureRectangle = new Rectangle(120, 216, 24, 24);
    }

}
