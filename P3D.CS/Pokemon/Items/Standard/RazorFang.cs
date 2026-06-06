using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(183, "Razor Fang")]
public class RazorFang : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It may make foes && allies flinch when the holder inflicts damage.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int BattlePointsPrice { get; } = 48;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public RazorFang()
    {
        _textureRectangle = new Rectangle(456, 144, 24, 24);
    }

}
