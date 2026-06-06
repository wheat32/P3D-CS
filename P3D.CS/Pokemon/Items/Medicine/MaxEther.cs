using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(67, "Max Ether")]
public class MaxEther : MedicineItem
{
    public override String Description { get; protected set; } = "Restores all PP to one move.";
    public override bool CanBeUsedInBattle { get; } = true;

    public MaxEther()
    {
        _textureRectangle = new Rectangle(456, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 6: PP restore logic (requires ChooseAttackScreen)
        return false;
    }
}
