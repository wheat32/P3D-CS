using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(314, "Bloom Mail")]
public class BloomMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of pretty floral patterns. Let a Pokémon hold it for delivery.";
    public BloomMail()
    {
        _textureRectangle = new Rectangle(312, 456, 24, 24);
    }

    public override void Use()
    {
        String MailID = "";
        if (IsGameModeItem == true)
        {
            MailID = gmID;
        }
        else
        {
            MailID = ID.ToString();
        }
        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MailSystemScreen(Core.CurrentScreen, MailID), Color.Black, false));
    }

}
