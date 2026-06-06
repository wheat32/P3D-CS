using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(143, "Metal Coat")]
public class MetalCoat : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a special metallic film that can boost the power of Steel-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public MetalCoat()
    {
        _textureRectangle = new Rectangle(408, 120, 24, 24);
    }

}
