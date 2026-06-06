using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(282, "Toxic Plate")]
public class ToxicPlate : PlateItem
{
    public ToxicPlate() : base(Element.Types.Poison)
    {
    }

}
