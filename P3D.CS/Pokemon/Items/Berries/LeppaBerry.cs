using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2011, "Leppa")]
public class LeppaBerry : Berry
{
    public LeppaBerry() : base(3600, "A berry that restores 10 PP of one move.", "2.0cm", "Soft", 2, 3)
    {
        type = (int)Element.Types.Normal;
        Power = 80;
    }

    public override void Use()
    {
        // TODO Phase 6: PP restore berry logic
    }
}
