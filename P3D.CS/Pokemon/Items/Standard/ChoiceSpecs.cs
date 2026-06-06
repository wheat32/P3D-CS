using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(580, "Choice Specs")]
public class ChoiceSpecs : Item
{
    public override String Description { get; protected set; } = "Boosts Special Attack by 50%, but only allows the use of the first move selected.";
    public override int PokeDollarPrice { get; protected set; } = 4000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName => Name;
    public ChoiceSpecs()
    {
        _textureRectangle = new Rectangle(48, 288, 24, 24);
    }

}
