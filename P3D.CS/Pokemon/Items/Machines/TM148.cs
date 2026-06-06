using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(448, "TM 148")]
public class TM148 : TechMachine
{
    public TM148() : base(true, 3000, 490)
    {
    }

}
