using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(576, "Mega Bracelet")]
public class MegaBracelet : KeyItem
{
    public override String Description { get; protected set; } = "This bracelet contains an untold power that somehow enables Pokémon carrying a Mega Stone to Mega Evolve in battle.";
    public MegaBracelet()
    {
        _textureRectangle = new Rectangle(312, 288, 24, 24);
    }

}
