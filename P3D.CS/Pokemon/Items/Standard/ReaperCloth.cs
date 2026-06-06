using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(84, "Reaper Cloth")]
public class ReaperCloth : Item
{
    public override String Description { get; protected set; } = "A cloth imbued with horrifyingly strong spiritual energy. It's loved by a certain Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ReaperCloth()
    {
        _textureRectangle = new Rectangle(96, 168, 24, 24);
    }

}
