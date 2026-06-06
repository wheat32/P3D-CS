using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(281, "Stone Plate")]
public class StonePlate : PlateItem
{
    public StonePlate() : base(Element.Types.Rock)
    {
    }

}
