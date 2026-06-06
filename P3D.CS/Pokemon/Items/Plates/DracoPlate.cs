using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(267, "Draco Plate")]
public class DracoPlate : PlateItem
{
    public DracoPlate() : base(Element.Types.Dragon)
    {
    }

}
