using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(106, "Smoke Ball")]
public class SmokeBall : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It enables the holder to flee from any wild Pokémon encounter without fail.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SmokeBall()
    {
        _textureRectangle = new Rectangle(192, 96, 24, 24);
    }

}
