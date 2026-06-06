using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2052, "Liechi")]
public class LiechiBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public LiechiBerry() : base(86400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, its Attack stat will increase when it's in a pinch.", "11.1cm", "Very Hard", 1, 2)
    {

        Spicy = 30;
        Dry = 10;
        Sweet = 30;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Grass;
        Power = 100;
        JuiceColor = "red";
        JuiceGroup = 3;
    }

}
