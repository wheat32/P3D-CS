using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(6, "Bicycle")]
public class Bicycle : KeyItem
{
    public override String Description { get; protected set; } = "A folding Bicycle.";
    public override bool CanBeUsed { get; } = true;

    public Bicycle()
    {
        _textureRectangle = new Rectangle(120, 0, 24, 24);
    }

    public override void Use()
    {
        if (Core.CurrentScreen.Identification != Screen.Identifications.OverworldScreen)
            return;
        Level level = Screen.Level!;
        if (level.Riding == true)
        {
            level.Riding = false;
            Core.Player.TempRideSkin = String.Empty;
            if (level.IsRadioOn == false)
            {
                if (level.Surfing == true)
                    MusicManager.Play("surf", true);
                else if (MusicManager.GetSong(level.MusicLoop) != null)
                    MusicManager.Play(level.MusicLoop, true, 0.01f);
                else
                    MusicManager.Play("silence");
            }
        }
        else if (level.CanRide() == true)
        {
            level.Riding = true;
            Core.Player.TempRideSkin = Core.Player.Skin;
            MusicManager.Play("ride", true);
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_Here", "Can't use that here!"), []);
        }
    }
}
