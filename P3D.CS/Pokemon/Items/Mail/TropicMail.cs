using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(310, "Tropic Mail")]
public class TropicMail : MailItem
{
    public override String Description { get; protected set; } = "A Bellossom-print Mail to be held by a Pokémon.";
    public TropicMail()
    {
        _textureRectangle = new Rectangle(216, 456, 24, 24);
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
