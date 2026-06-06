using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(269, "Earth Plate")]
public class EarthPlate : PlateItem
{
    public EarthPlate() : base(Element.Types.Ground)
    {
    }

}
