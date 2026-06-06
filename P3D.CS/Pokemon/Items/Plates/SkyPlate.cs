using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(278, "Sky Plate")]
public class SkyPlate : PlateItem
{
    public SkyPlate() : base(Element.Types.Flying)
    {
    }

}
