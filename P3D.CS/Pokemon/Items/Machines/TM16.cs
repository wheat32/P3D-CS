using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(206, "TM 16")]
public class TM16 : TechMachine
{
    public TM16() : base(true, 3000, 196)
    {
    }

}
