using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(102, "Black Glasses")]
public class BlackGlasses : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokemon. A pair of shady-looking glasses that boost the power of Dark-type moves.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Black Glasses";
    public BlackGlasses()
    {
        _textureRectangle = new Rectangle(96, 96, 24, 24);
    }

}
