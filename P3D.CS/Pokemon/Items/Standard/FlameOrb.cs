using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(577, "Flame Orb")]
public class FlameOrb : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a bizarre orb that inflicts a burn on the holder in battle.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public FlameOrb()
    {
        _textureRectangle = new Rectangle(480, 264, 24, 24);
    }

}
