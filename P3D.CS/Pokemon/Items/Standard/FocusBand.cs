using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(119, "Focus Band")]
public class FocusBand : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. The holder may endure a potential KO attack, leaving it with just 1 HP.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int BattlePointsPrice { get; } = 64;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public FocusBand()
    {
        _textureRectangle = new Rectangle(480, 96, 24, 24);
    }

}
