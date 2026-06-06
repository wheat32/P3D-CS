using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(654, "Peat Block")]
public class PeatBlock : StoneItem
{
    public override String Description { get; protected set; } = "A block of muddy material that can be used as fuel for burning when it == dried. It’s loved by a certain Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 1000;
    public override int FlingDamage { get; } = 80;
    public PeatBlock()
    {
        _textureRectangle = new Rectangle(408, 408, 24, 24);
    }

}
