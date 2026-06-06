using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(600, "Terrain Extender")]
public class TerrainExtender : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It extends the duration of the terrain caused by the holder's move || Ability.";
    public override int PokeDollarPrice { get; protected set; } = 2000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public TerrainExtender()
    {
        _textureRectangle = new Rectangle(288, 312, 24, 24);
    }

}
