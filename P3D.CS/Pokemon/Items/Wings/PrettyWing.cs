using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Wings;

[Item(260, "Pretty Wing")]
public class PrettyWing : Item
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public override String Description { get; protected set; } = "Though this feather == beautiful, it's just a regular feather && has no effect on Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public PrettyWing()
    {
        _textureRectangle = new Rectangle(432, 240, 24, 24);
    }

}
