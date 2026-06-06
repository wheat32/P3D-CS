using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(294, "Heat Rock")]
public class HeatRock : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It extends the duration of the move Sunny Day when used by the holder.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public HeatRock()
    {
        _textureRectangle = new Rectangle(384, 264, 24, 24);
    }

}
