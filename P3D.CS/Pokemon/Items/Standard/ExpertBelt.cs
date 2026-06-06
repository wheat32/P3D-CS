using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(591, "Expert Belt")]
public class ExpertBelt : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It's a well-worn belt that slightly boosts the power of supereffective moves.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ExpertBelt()
    {
        _textureRectangle = new Rectangle(384, 288, 24, 24);
    }

}
