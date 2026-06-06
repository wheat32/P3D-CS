using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(303, "Fab Mail")]
public class FabMail : MailItem
{
    public override String Description { get; protected set; } = "A gorgeous-print Mail to be held by a Pokémon.";
    public FabMail()
    {
        _textureRectangle = new Rectangle(48, 456, 24, 24);
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
