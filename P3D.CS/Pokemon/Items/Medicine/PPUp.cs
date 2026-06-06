using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(68, "PP Up")]
public class PPUp : MedicineItem
{
    public override String Description { get; protected set; } = "Raises max PP of one move.";
    public override bool CanBeUsedInBattle { get; } = true;

    public PPUp()
    {
        _textureRectangle = new Rectangle(480, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 6: PP max raise logic (requires ChooseAttackScreen)
        return false;
    }
}
