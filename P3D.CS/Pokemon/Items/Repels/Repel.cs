using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Repels;

[Item(20, "Repel")]
public class Repel : RepelItem
{
    public override String Description { get; protected set; } = "An item that prevents any low-level wild Pokémon from jumping out at you for 100 steps after its use.";
    public override int PokeDollarPrice { get; protected set; } = 350;
    public override int RepelSteps { get; } = 100;
    public Repel()
    {
        _textureRectangle = new Rectangle(432, 0, 24, 24);
    }

}
