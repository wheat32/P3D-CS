using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(148, "Soothe Bell")]
public class SootheBell : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. The comforting chime of this bell calms the holder, making it friendly.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public SootheBell()
    {
        _textureRectangle = new Rectangle(0, 216, 24, 24);
    }

}
