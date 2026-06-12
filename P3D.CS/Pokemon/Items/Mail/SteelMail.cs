using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(322, "Steel Mail")]
public class SteelMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of cool mechanical designs. Let a Pokémon hold it for delivery.";
    public SteelMail()
    {
        _textureRectangle = new Rectangle(24, 480, 24, 24);
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
