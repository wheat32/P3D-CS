using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(296, "Icy Rock")]
public class IcyRock : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It extends the duration of the move Hail when used by the holder.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public IcyRock()
    {
        _textureRectangle = new Rectangle(432, 264, 24, 24);
    }

}
