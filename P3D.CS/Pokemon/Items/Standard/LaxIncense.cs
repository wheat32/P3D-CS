using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(289, "Lax Incense")]
public class LaxIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. The beguiling aroma of this incense may cause attacks to miss its holder.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LaxIncense()
    {
        _textureRectangle = new Rectangle(240, 264, 24, 24);
    }

}
