using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(57, "Exp. Share")]
public class ExpShare : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. The holder gets a share of a battle's Exp. Points without battling.";
    public override int PokeDollarPrice { get; protected set; } = 3000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ExpShare()
    {
        _textureRectangle = new Rectangle(216, 48, 24, 24);
    }

}
