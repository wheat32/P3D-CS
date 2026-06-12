using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(334, "Like Mail")]
public class LikeMail : MailItem
{
    public override String Description { get; protected set; } = "Stationary designed for writing recommendations. Let a Pokémon hold it for delivery.";
    public LikeMail()
    {
        _textureRectangle = new Rectangle(312, 480, 24, 24);
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
