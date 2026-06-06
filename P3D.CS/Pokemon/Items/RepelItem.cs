namespace P3D.Items.Repels;

public abstract class RepelItem : Item
{
    public override bool CanBeUsedInBattle { get; } = false;

    public abstract int RepelSteps { get; }

    public override void Use()
    {
        if (Core.Player.RepelSteps <= 0)
        {
            Player.Temp.LastUsedRepel = ID;
            SoundManager.PlaySound("Use_Repel", false);
            Core.Player.RepelSteps = RepelSteps;
            PlayerStatistics.Track("[42]Repels used", 1);
            String t = Core.Player.Name + " used a~" + Name + ".";
            t += RemoveItem();
            Screen.TextBox.Show(t, [], true, true);
        }
        else
        {
            Screen.TextBox.Show("The Repel is still~in effect.", [], true, true);
        }
    }
}
