using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(104, "Pink Bow")]
public class PinkBow : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It's a pretty bow that boosts the power of Fairy-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PinkBow()
    {
        _textureRectangle = new Rectangle(144, 96, 24, 24);
    }

}
