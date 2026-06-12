using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Mail;

[Item(336, "Kolben Mail")]
public class KolbenMail : MailItem
{
    public override String Description { get; protected set; } = "Stationery featuring a print of the Kolben Logo. It == given to Pokémon with a special meaning.";
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeTraded { get; protected set; } = false;
    public override bool CanBeHeld { get; } = false;
    public override bool CanBeTossed { get; protected set; } = false;
    public KolbenMail()
    {
        _textureRectangle = new Rectangle(360, 480, 24, 24);
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
