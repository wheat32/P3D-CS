namespace P3D.Items;

public abstract class VitaminItem : Item
{
    public override ItemTypes ItemType { get; } = ItemTypes.Medicine;
    public override int PokeDollarPrice { get; protected set; } = 9800;
    public override bool CanBeUsedInBattle { get; } = false;

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

    protected static bool CanUseVitamin(int stat, Pokemon p)
    {
        if (stat < 100)
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
