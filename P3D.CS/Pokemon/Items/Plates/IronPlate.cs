using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(274, "Iron Plate")]
public class IronPlate : PlateItem
{
    public IronPlate() : base(Element.Types.Steel)
    {
    }

}
