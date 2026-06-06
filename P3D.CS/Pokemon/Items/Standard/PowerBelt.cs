using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(584, "Power Belt")]
public class PowerBelt : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It reduces Speed but allows the holder's Defense stat to grow more after battling.";
    public override int PokeDollarPrice { get; protected set; } = 3000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PowerBelt()
    {
        _textureRectangle = new Rectangle(168, 288, 24, 24);
    }

}
