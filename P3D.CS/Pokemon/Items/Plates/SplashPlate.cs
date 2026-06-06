using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plates;

[Item(279, "Splash Plate")]
public class SplashPlate : PlateItem
{
    public SplashPlate() : base(Element.Types.Water)
    {
    }

}
