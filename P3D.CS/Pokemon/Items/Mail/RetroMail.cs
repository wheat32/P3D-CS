using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(308, "Retro Mail")]
public class RetroMail : MailItem
{
    public override String Description { get; protected set; } = "Mail featuring the drawings of three Pokémon.";
    public RetroMail()
    {
        _textureRectangle = new Rectangle(168, 456, 24, 24);
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
