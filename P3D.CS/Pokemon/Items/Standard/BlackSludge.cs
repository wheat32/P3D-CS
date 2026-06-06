using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(182, "Black Sludge")]
public class BlackSludge : Item
{
    public override String Description { get; protected set; } = "A held item that gradually restores the HP of Poison-type Pokémon. It inflicts damage on all other types.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BlackSludge()
    {
        _textureRectangle = new Rectangle(432, 144, 24, 24);
    }

}
