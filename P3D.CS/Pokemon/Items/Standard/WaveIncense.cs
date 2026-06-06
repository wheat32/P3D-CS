using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(145, "Wave Incense")]
public class WaveIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This exotic-smelling incense boots the power of Water-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public WaveIncense()
    {
        _textureRectangle = new Rectangle(480, 192, 24, 24);
    }

}
