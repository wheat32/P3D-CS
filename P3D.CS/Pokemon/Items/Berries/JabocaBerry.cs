using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2062, "Jaboca")]
public class JabocaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public JabocaBerry() : base(86400, "If held by a Pokémon, && if a foe's physical attack lands, the foe also takes damage.", "3.3cm", "Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 0;
        Bitter = 40;
        Sour = 10;

        type = (int)Element.Types.Dragon;
        Power = 100;
        JuiceColor = "yellow";
        JuiceGroup = 3;
    }

}
