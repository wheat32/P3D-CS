using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(83, "Prism Scale")]
public class PrismScale : Item
{
    public override String Description { get; protected set; } = "A mysterious scale that causes a certain Pokémon to evolve. It shines in rainbow colors.";
    public override int PokeDollarPrice { get; protected set; } = 500;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public PrismScale()
    {
        _textureRectangle = new Rectangle(72, 168, 24, 24);
    }

}
