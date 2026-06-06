using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(94, "CleanseTag")]
public class CleanseTag : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It helps keep wild Pokémon away if the holder == the head of the party.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public CleanseTag()
    {
        _textureRectangle = new Rectangle(432, 72, 24, 24);
    }

}
