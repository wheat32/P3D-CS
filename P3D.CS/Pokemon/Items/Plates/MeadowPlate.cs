using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(275, "Meadow Plate")]
public class MeadowPlate : PlateItem
{
    public MeadowPlate() : base(Element.Types.Grass)
    {
    }

}
