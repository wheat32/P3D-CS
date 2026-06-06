using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Repels;

[Item(43, "Max Repel")]
public class MaxRepel : RepelItem
{
    public override String Description { get; protected set; } = "An item that prevents any low-level wild Pokémon from jumping out at you for 250 steps after its use.";
    public override int PokeDollarPrice { get; protected set; } = 700;
    public override int RepelSteps { get; } = 250;
    public MaxRepel()
    {
        _textureRectangle = new Rectangle(456, 24, 24, 24);
    }

}
