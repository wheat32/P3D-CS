using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(439, "TM 139")]
public class TM139 : TechMachine
{
    public TM139() : base(true, 3000, 430)
    {
    }

}
