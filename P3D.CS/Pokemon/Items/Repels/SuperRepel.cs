using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Repels;

[Item(42, "Super Repel")]
public class SuperRepel : RepelItem
{
    public override String Description { get; protected set; } = "An item that prevents any low-level wild Pokémon from jumping out at you for 200 steps after its use.";
    public override int PokeDollarPrice { get; protected set; } = 500;
    public override int RepelSteps { get; } = 200;
    public SuperRepel()
    {
        _textureRectangle = new Rectangle(432, 24, 24, 24);
    }

}
