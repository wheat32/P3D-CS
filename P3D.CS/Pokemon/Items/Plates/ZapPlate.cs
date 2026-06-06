using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(283, "Zap Plate")]
public class ZapPlate : PlateItem
{
    public ZapPlate() : base(Element.Types.Electric)
    {
    }

}
