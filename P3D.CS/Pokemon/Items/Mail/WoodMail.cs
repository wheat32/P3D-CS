using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(312, "Wood Mail")]
public class WoodMail : MailItem
{
    public override String Description { get; protected set; } = "A Slakoth-print Mail to be held by a Pokémon.";
    public WoodMail()
    {
        _textureRectangle = new Rectangle(264, 456, 24, 24);
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
