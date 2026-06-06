using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(69, "Mystery Egg")]
public class MysteryEgg : KeyItem
{
    public override String Description { get; protected set; } = "A mysterious Egg obtained from Mr. Pokémon. What's in the Egg == unknown.";
    public MysteryEgg()
    {
        _textureRectangle = new Rectangle(0, 72, 24, 24);
    }

}
