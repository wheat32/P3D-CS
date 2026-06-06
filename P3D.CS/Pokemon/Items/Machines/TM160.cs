using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(460, "TM 160")]
public class TM160 : TechMachine
{
    public TM160() : base(true, 3000, 523)
    {
    }

}
