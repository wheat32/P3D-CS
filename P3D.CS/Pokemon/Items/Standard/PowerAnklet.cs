using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(587, "Power Anklet")]
public class PowerAnklet : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It reduces Speed but allows the holder's Speed stat to grow more after battling.";
    public override int PokeDollarPrice { get; protected set; } = 3000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PowerAnklet()
    {
        _textureRectangle = new Rectangle(240, 288, 24, 24);
    }

}
