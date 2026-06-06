using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(505, "Toxic Orb")]
public class ToxicOrb : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It == a bizarre orb that will badly poison the holder during battle.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override int BattlePointsPrice { get; } = 16;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ToxicOrb()
    {
        _textureRectangle = new Rectangle(216, 240, 24, 24);
    }

}
