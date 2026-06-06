using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(291, "Pure Incense")]
public class PureIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It helps keep wild Pokémon away if the holder == the head of the party.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PureIncense()
    {
        _textureRectangle = new Rectangle(288, 264, 24, 24);
    }

}
