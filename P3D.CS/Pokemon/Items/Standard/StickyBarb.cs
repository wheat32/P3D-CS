using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(70, "Sticky Barb")]
public class StickyBarb : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It damages the holder every turn && may latch on to Pokémon that touch the holder.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 80;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public StickyBarb()
    {
        _textureRectangle = new Rectangle(24, 168, 24, 24);
    }

}
