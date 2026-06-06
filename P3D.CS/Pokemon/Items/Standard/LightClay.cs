using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(297, "Light Clay")]
public class LightClay : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. Protective moves like Light Screen && Reflect will be effective longer.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LightClay()
    {
        _textureRectangle = new Rectangle(456, 264, 24, 24);
    }

}
