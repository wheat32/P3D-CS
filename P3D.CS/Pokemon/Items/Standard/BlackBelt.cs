using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(98, "Black Belt")]
public class BlackBelt : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This belt helps the wearer to focus && boosts the power of Fighting-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BlackBelt()
    {
        _textureRectangle = new Rectangle(24, 96, 24, 24);
    }

}
