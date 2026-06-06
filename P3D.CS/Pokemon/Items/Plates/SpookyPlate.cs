using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(280, "Spooky Plate")]
public class SpookyPlate : PlateItem
{
    public SpookyPlate() : base(Element.Types.Ghost)
    {
    }

}
