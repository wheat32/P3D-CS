using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(88, "SilverPowder")]
public class SilverPowder : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a shiny, silver powder that will boost the power of Bug-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SilverPowder()
    {
        _textureRectangle = new Rectangle(312, 72, 24, 24);
    }

}
