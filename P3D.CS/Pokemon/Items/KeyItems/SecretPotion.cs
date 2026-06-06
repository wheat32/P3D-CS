using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(67, "SecretPotion")]
public class SecretPotion : KeyItem
{
    public override String Description { get; protected set; } = "A fantastic medicine dispensed by the pharmacy in Cianwood City. It fully heals a Pokémon of any ailment.";
    public SecretPotion()
    {
        _textureRectangle = new Rectangle(456, 48, 24, 24);
    }

}
