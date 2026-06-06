using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(579, "Choice Scarf")]
public class ChoiceScarf : Item
{
    public override String Description { get; protected set; } = "Boosts Speed by 50%, but only allows the use of the first move selected.";
    public override int PokeDollarPrice { get; protected set; } = 4000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Choice Scarves";
    public ChoiceScarf()
    {
        _textureRectangle = new Rectangle(24, 288, 24, 24);
    }

}
