using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2014, "Iapapa")]
public class IapapaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public IapapaBerry() : base(21600, "If held by a Pokémon, it restores the user's HP in a pinch, but it will cause confusion if the user hates the taste.", "22.3cm", "Soft", 2, 3)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 15;

        type = (int)Element.Types.Dark;
        Power = 80;
        JuiceColor = "yellow";
        JuiceGroup = 3;
    }

}
