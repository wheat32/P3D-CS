namespace P3D.Items;

public abstract class WingItem : Item
{
    public override ItemTypes ItemType { get; } = ItemTypes.Medicine;
    public override bool CanBeUsedInBattle { get; } = false;
    public override int FlingDamage { get; } = 20;
    public override int PokeDollarPrice { get; protected set; } = 3000;

    public override void Use()
    {
        PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, this, UseOnPokemon,
            Localization.GetString("global_use", "Use") + " " + OneLineName(), true)
        {
            Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
            CanExit = true,
        };
        selScreen.SelectedObject += UseItemhandler;
        Core.SetScreen(selScreen);
    }

    protected static bool CanUseWing(int stat, Pokemon p)
    {
        if (stat < 255)
        {
            int allStats = p.EVAttack + p.EVDefense + p.EVSpAttack + p.EVSpDefense + p.EVHP + p.EVSpeed;
            if (allStats < 510)
            {
                return true;
            }
        }
        return false;
    }
}
