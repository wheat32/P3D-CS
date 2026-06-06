using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(431, "TM 131")]
public class TM131 : TechMachine
{
    public TM131() : base(true, 1500, 445)
    {
        CanTeachWhenGender = true;
    }

}
