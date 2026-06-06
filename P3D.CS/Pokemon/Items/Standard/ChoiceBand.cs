using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(578, "Choice Band")]
public class ChoiceBand : Item
{
    public override String Description { get; protected set; } = "Boosts Attack by 50%, but only allows the use of the first move selected.";
    public override int PokeDollarPrice { get; protected set; } = 4000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ChoiceBand()
    {
        _textureRectangle = new Rectangle(0, 288, 24, 24);
    }

}
