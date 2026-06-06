using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(201, "TM 11")]
public class TM11 : TechMachine
{
    public TM11() : base(true, 2000, 241)
    {
    }

}
