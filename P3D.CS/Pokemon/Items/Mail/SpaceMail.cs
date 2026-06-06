using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(321, "Space Mail")]
public class SpaceMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print depicting the huge expanse of space. Let a Pokémon hold it for delivery.";
    public SpaceMail()
    {
        _textureRectangle = new Rectangle(0, 480, 24, 24);
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
