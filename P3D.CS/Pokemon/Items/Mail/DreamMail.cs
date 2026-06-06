using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(302, "Dream Mail")]
public class DreamMail : MailItem
{
    public override String Description { get; protected set; } = "Mail featuring a sketch of the holding Pokémon.";
    public DreamMail()
    {
        _textureRectangle = new Rectangle(24, 456, 24, 24);
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
