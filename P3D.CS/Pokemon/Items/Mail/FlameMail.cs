using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(317, "Flame Mail")]
public class FlameMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of flames in blazing red. Let a Pokémon hold it for delivery.";
    public FlameMail()
    {
        _textureRectangle = new Rectangle(384, 456, 24, 24);
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
