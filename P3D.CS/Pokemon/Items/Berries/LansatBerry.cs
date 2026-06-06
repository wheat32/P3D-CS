using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2057, "Lansat")]
public class LansatBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public LansatBerry() : base(86400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, its critical-hit ratio will increase when it's in a pinch.", "9.7cm", "Soft", 1, 2)
    {

        Spicy = 30;
        Dry = 10;
        Sweet = 30;
        Bitter = 10;
        Sour = 30;

        type = (int)Element.Types.Flying;
        Power = 100;
        JuiceColor = "red";
        JuiceGroup = 3;
    }

}
