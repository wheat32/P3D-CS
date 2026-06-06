using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(144, "Dragon Fang")]
public class DragonFang : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. This hard && sharp fang boosts the power of Dragon-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 70;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public DragonFang()
    {
        _textureRectangle = new Rectangle(432, 120, 24, 24);
    }

}
