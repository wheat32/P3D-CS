using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(112, "Poké Doll")]
public class PokéDoll : Item
{
    public override String Description { get; protected set; } = "Helps flee from a battle.";
    public override bool CanBeUsedInBattle { get; } = true;

    public PokéDoll()
    {
        _textureRectangle = new Rectangle(0, 120, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 5: escape from battle logic (requires BattleScreen)
        return false;
    }
}
