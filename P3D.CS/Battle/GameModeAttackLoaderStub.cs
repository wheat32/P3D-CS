namespace P3D.BattleSystem;

// TODO Phase 5: full GameModeAttackLoader port
public static partial class GameModeAttackLoader
{
    public static Attack GetAttackByID(int id)
    {
        Attack a = new Attack();
        a.isDefaultMove = true;
        a.originalID = id;
        return a;
    }
}
