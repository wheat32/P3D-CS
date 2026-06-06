using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(581, "Macho Brace")]
public class MachoBrace : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This stiff, heavy brace helps Pokemon grow strong but cuts Speed in battle.";
    public override int PokeDollarPrice { get; protected set; } = 3000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public MachoBrace()
    {
        _textureRectangle = new Rectangle(96, 288, 24, 24);
    }

}
