using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(276, "Mind Plate")]
public class MindPlate : PlateItem
{
    public MindPlate() : base(Element.Types.Psychic)
    {
    }

}
