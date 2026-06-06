using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(402, "TM 102")]
public class TM102 : TechMachine
{
    public TM102() : base(true, 2000, 332)
    {
    }

}
