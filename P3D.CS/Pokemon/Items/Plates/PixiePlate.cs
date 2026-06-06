using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(277, "Pixie Plate")]
public class PixiePlate : PlateItem
{
    public PixiePlate() : base(Element.Types.Fairy)
    {
    }

}
