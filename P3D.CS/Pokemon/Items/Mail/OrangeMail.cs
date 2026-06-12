using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(307, "Orange Mail")]
public class OrangeMail : MailItem
{
    public override String Description { get; protected set; } = "A Zigzagoon-print Mail to be held by a Pokémon.";
    public OrangeMail()
    {
        _textureRectangle = new Rectangle(144, 456, 24, 24);
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
