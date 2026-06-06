using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(455, "TM 155")]
public class TM155 : TechMachine
{
    public TM155() : base(true, 3000, 511)
    {
    }

}
