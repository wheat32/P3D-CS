using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(301, "Bead Mail")]
public class BeadMail : MailItem
{
    public override String Description { get; protected set; } = "Mail featuring a sketch of the holding Pokémon.";
    public BeadMail()
    {
        _textureRectangle = new Rectangle(0, 456, 24, 24);
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
