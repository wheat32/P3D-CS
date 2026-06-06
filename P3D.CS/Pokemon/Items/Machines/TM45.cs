using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(235, "TM 45")]
public class TM45 : TechMachine
{
    public override int BattlePointsPrice { get; } = 32;
    public TM45() : base(true, 1500, 213)
    {
        CanTeachWhenGender = true;
    }

}
