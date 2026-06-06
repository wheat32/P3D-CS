using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(388, "TM 88")]
public class TM88 : TechMachine
{
    public TM88() : base(true, 3000, 264)
    {
    }

}
