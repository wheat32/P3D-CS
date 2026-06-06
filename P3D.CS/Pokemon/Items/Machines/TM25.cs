using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(215, "TM 25")]
public class TM25 : TechMachine
{
    public TM25() : base(true, 5500, 87)
    {
    }

}
