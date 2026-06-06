using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(91, "Amulet Coin")]
public class AmuletCoin : Item
{
    public override String Description { get; protected set; } = "An item to be held by a Pokémon. It doubles any prize money received if the holding Pokémon joins in a battle.";
    public override int PokeDollarPrice { get; protected set; } = 100;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public AmuletCoin()
    {
        _textureRectangle = new Rectangle(360, 72, 24, 24);
    }

}
