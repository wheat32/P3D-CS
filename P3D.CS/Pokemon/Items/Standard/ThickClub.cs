using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(118, "Thick Club")]
public class ThickClub : Item
{
    public override String Description { get; protected set; } = "A rare bone that == extremely valuable for the study of Pokémon archeology. It can be sold for a high price to shops.";
    public override int PokeDollarPrice { get; protected set; } = 500;
    public override int FlingDamage { get; } = 90;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public ThickClub()
    {
        _textureRectangle = new Rectangle(456, 96, 24, 24);
    }

}
