using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(272, "Icicle Plate")]
public class IciclePlate : PlateItem
{
    public IciclePlate() : base(Element.Types.Ice)
    {
    }

}
