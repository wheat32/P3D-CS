using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(503, "Sachet")]
public class Sachet : Item
{
    public override String Description { get; protected set; } = "A sachet filled with fragrant perfumes that are just slightly too overwhelming. Yet it's loved by a certain Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Sachet()
    {
        _textureRectangle = new Rectangle(144, 240, 24, 24);
    }

}
