using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(74, "Zoom Lens")]
public class ZoomLens : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. If the holder moves after its target moves, its accuracy will be boosted.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Zoom Lenses";
    public ZoomLens()
    {
        _textureRectangle = new Rectangle(264, 144, 24, 24);
    }

}
