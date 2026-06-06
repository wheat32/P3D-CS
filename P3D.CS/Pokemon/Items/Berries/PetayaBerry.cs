using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2055, "Petaya")]
public class PetayaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public PetayaBerry() : base(86400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, its Sp. Atk. stat will increase when it's in a pinch.", "23.7cm", "Very Hard", 1, 2)
    {

        Spicy = 30;
        Dry = 0;
        Sweet = 0;
        Bitter = 30;
        Sour = 10;

        type = (int)Element.Types.Poison;
        Power = 100;
        JuiceColor = "pink";
        JuiceGroup = 3;
    }

}
