using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(227, "TM 37")]
public class TM37 : TechMachine
{
    public TM37() : base(true, 2000, 201)
    {
    }

}
