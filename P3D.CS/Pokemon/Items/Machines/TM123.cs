using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(423, "TM 123")]
public class TM123 : TechMachine
{
    public TM123() : base(true, 1000, 278)
    {
    }

}
