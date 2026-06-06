using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(475, "TM 175")]
public class TM175 : TechMachine
{
    public TM175() : base(true, 2000, 270)
    {
    }

}
