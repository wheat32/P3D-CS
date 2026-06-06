using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(389, "TM 89")]
public class TM89 : TechMachine
{
    public TM89() : base(true, 3000, 337)
    {
    }

}
