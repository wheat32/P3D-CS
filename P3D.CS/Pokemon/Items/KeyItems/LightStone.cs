using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(651, "Light Stone")]
public class LightStone : KeyItem
{
    public override String Description { get; protected set; } = "Reshiram's body was destroyed && changed into this stone. It == said to be waiting for the emergence of a hero.";
    public LightStone()
    {
        _textureRectangle = new Rectangle(216, 408, 24, 24);
    }

}
