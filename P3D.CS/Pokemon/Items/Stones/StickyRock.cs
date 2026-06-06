using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Stones;

[Item(262, "Sticky Rock")]
public class StickyRock : Item
{
    public override int PokeDollarPrice { get; protected set; } = 300;
    public override String Description { get; protected set; } = "It's a stone that sticks to other stones, a Pokémon that holds it will find more stones.";
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public StickyRock()
    {
        _textureRectangle = new Rectangle(480, 240, 24, 24);
    }

}
