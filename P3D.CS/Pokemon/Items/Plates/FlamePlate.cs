using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(271, "Flame Plate")]
public class FlamePlate : PlateItem
{
    public FlamePlate() : base(Element.Types.Fire)
    {
    }

}
