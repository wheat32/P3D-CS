using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(290, "Luck Incense")]
public class LuckIncense : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It doubles a battle's prize money if the holding Pokémon joins in.";
    public override int PokeDollarPrice { get; protected set; } = 9600;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public LuckIncense()
    {
        _textureRectangle = new Rectangle(264, 264, 24, 24);
    }

}
