using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(82, "King's Rock")]
public class KingsRock : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. When the holder successfully inflicts damage, the target may also flinch.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int BattlePointsPrice { get; } = 64;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public KingsRock()
    {
        _textureRectangle = new Rectangle(216, 72, 24, 24);
    }

}
