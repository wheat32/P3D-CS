using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(175, "SquirtBottle")]
public class Squirtbottle : KeyItem
{
    public override String Description { get; protected set; } = "A bottle used for watering plants in Loamy Soil.";
    public Squirtbottle()
    {
        _textureRectangle = new Rectangle(360, 144, 24, 24);
    }

}
