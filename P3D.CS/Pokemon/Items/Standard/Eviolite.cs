using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(298, "Eviolite")]
public class Eviolite : Item
{
    public override String Description { get; protected set; } = "A mysterious Evolutionary lump. When held by a Pokémon that can still evolve, it raises both Defense && Sp. Def.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public Eviolite()
    {
        _textureRectangle = new Rectangle(336, 288, 24, 24);
    }

}
