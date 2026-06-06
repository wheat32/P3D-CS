using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(54, "Coin Case")]
public class CoinCase : KeyItem
{
    public override String Description { get; protected set; } = "A case for holding coins obtained at the Game Corner.";
    public override bool CanBeUsed { get; } = true;
    public CoinCase()
    {
        _textureRectangle = new Rectangle(168, 48, 24, 24);
    }

    public override void Use()
    {
        Screen.TextBox.Show(Localization.GetString("item_use_54", "Your coins:~") + Core.Player.Coins, [], true, true);
    }

}
