using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(300, "Grass Mail")]
public class GrassMail : MailItem
{
    public override String Description { get; protected set; } = "Let a Pokémon hold it for delivery.";
    public GrassMail()
    {
        _textureRectangle = new Rectangle(408, 456, 24, 24);
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
