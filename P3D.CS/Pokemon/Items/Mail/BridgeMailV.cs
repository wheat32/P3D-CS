using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(327, "BridgeMail V")]
public class BridgeMailV : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of a brick bridge. Let a Pokémon hold it for use.";
    public BridgeMailV()
    {
        _textureRectangle = new Rectangle(144, 480, 24, 24);
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
