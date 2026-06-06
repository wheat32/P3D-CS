using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(273, "Insect Plate")]
public class InsectPlate : PlateItem
{
    public InsectPlate() : base(Element.Types.Bug)
    {
    }

}
