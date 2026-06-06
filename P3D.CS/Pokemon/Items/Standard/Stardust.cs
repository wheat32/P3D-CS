using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(131, "Stardust")]
public class Stardust : Item
{
    public override String Description { get; protected set; } = "Lovely, red sand that flows between the fingers with a loose, silky feel. It can be sold at a high price to shops.";
    public override int PokeDollarPrice { get; protected set; } = 2000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Stardust()
    {
        _textureRectangle = new Rectangle(240, 120, 24, 24);
    }

}
