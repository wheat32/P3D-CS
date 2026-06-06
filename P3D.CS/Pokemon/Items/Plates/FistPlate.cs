using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(270, "Fist Plate")]
public class FistPlate : PlateItem
{
    public FistPlate() : base(Element.Types.Fighting)
    {
    }

}
