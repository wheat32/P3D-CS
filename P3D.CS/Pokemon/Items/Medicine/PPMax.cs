using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(69, "PP Max")]
public class PPMax : MedicineItem
{
    public override String Description { get; protected set; } = "Raises max PP to maximum.";
    public override bool CanBeUsedInBattle { get; } = true;

    public PPMax()
    {
        _textureRectangle = new Rectangle(504, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 12: PP max logic (requires ChooseAttackScreen)
        return false;
    }
}
