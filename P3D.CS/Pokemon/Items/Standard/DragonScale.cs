using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(151, "Dragon Scale")]
public class DragonScale : Item
{
    public override String Description { get; protected set; } = "A very tough && inflexible scale. Dragon-type Pokémon may be holding this item when caught.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public DragonScale()
    {
        _textureRectangle = new Rectangle(480, 120, 24, 24);
    }

}
