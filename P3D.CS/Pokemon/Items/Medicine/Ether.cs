using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Medicine;

[Item(66, "Ether")]
public class Ether : MedicineItem
{
    public override String Description { get; protected set; } = "Restores 10 PP to one move.";
    public override bool CanBeUsedInBattle { get; } = true;

    public Ether()
    {
        _textureRectangle = new Rectangle(432, 48, 24, 24);
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        // TODO Phase 12: PP restore logic (requires ChooseAttackScreen)
        return false;
    }
}
