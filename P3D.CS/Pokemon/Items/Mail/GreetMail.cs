using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(332, "Greet Mail")]
public class GreetMail : MailItem
{
    public override String Description { get; protected set; } = "Stationary designed for introductory greetings. Let a Pokémon hold it for delivery.";
    public GreetMail()
    {
        _textureRectangle = new Rectangle(264, 480, 24, 24);
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
