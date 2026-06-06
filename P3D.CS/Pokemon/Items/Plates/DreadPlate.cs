using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(268, "Dread Plate")]
public class DreadPlate : PlateItem
{
    public DreadPlate() : base(Element.Types.Dark)
    {
    }

}
