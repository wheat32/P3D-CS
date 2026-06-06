using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(141, "Protector")]
public class Protector : Item
{
    public override String Description { get; protected set; } = "A protective item of some sort. It == extremely stiff && heavy. It == loved by a certain Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 2100;
    public override int FlingDamage { get; } = 80;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Protector()
    {
        _textureRectangle = new Rectangle(408, 192, 24, 24);
    }

}
