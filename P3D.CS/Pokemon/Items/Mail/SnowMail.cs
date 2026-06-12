using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(320, "Snow Mail")]
public class SnowMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of a chilly, snow-covered world. Let a Pokémon hold it for delivery.";
    public SnowMail()
    {
        _textureRectangle = new Rectangle(480, 456, 24, 24);
    }

    public override void Use()
    {
        String MailID = String.Empty;
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
