namespace P3D.Items;

// TODO Phase 7: full GameModeItem port (~1,617 lines in VB source)
public class GameModeItem : Item
{
    public override String Description { get; protected set; } = "";
    public BattleSystem.Attack gmTeachMove { get; set; } = BattleSystem.Attack.GetAttackByID(1);
}
