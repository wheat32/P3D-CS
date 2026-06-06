using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(108, "Magnet")]
public class Magnet : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a powerful magnet that boosts the power of Electric-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Magnet()
    {
        _textureRectangle = new Rectangle(240, 96, 24, 24);
    }

}
