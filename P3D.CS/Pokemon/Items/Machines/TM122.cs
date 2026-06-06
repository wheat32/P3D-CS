using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(422, "TM 122")]
public class TM122 : TechMachine
{
    public TM122() : base(true, 3000, 371)
    {
    }

}
