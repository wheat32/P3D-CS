namespace P3D.Items;

// TODO Phase 12: full GameModeItem port (~1,617 lines in VB source)
public class GameModeItem : Item
{
    public override String Description { get; protected set; } = String.Empty;
    public BattleSystem.Attack gmTeachMove { get; set; } = BattleSystem.Attack.GetAttackByID(1);
    public bool gmIsHM = false;
    public double gmExpMultiplier = -1.0;
    public bool gmOverrideTradeExp;
    public List<String>? gmUseOnOwnEffects;
    public List<String>? gmUseOnOppEffects;
    public int gmMegaPokemonNumber;

    public bool UseOnOppPokemon(BattleSystem.BattleScreen battleScreen) => false;
    public String gmID { get; set; } = String.Empty;
    public String CanTeach(Pokemon p) => String.Empty;
}
