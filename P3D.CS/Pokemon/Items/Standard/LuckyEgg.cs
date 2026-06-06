using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(126, "Lucky Egg")]
public class LuckyEgg : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == an egg filled with happiness that earns extra Exp. Points in battle.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LuckyEgg()
    {
        _textureRectangle = new Rectangle(120, 120, 24, 24);
    }

}
