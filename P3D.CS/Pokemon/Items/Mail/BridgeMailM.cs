using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(328, "BridgeMail M")]
public class BridgeMailM : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of an arched bridge. Let a Pokémon hold it for use.";
    public BridgeMailM()
    {
        _textureRectangle = new Rectangle(168, 480, 24, 24);
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
