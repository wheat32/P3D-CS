using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(583, "Power Bracer")]
public class PowerBracer : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It reduces Speed but allows the holder's Attack stat to grow more after battling.";
    public override int PokeDollarPrice { get; protected set; } = 3000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PowerBracer()
    {
        _textureRectangle = new Rectangle(144, 288, 24, 24);
    }

}
