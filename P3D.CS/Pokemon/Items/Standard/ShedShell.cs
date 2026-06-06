using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(154, "Shed Shell")]
public class ShedShell : Item
{
    public override String Description { get; protected set; } = "A tough, discarded carapace to be held by a Pokémon. It enables the holder to switch with a waiting Pokémon in battle.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ShedShell()
    {
        _textureRectangle = new Rectangle(72, 216, 24, 24);
    }

}
