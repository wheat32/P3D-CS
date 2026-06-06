using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(504, "Whipped Dream")]
public class WhippedDream : Item
{
    public override String Description { get; protected set; } = "A soft && sweet treat made of fluffy, puffy, whipped && whirled cream. It's loved by a certain Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public WhippedDream()
    {
        _textureRectangle = new Rectangle(168, 240, 24, 24);
    }

}
