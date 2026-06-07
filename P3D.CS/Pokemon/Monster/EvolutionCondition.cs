namespace P3D;

// TODO Phase 3: full EvolutionCondition port
public class EvolutionCondition
{
    public enum EvolutionTrigger
    {
        LevelUp,
        Trade,
        Trading = Trade,
        UseItem,
        ItemUse = UseItem,
        Happiness,
        None,
        Other
    }

    public static String EvolutionNumber(Pokemon pokemon, EvolutionTrigger trigger, String argument)
    {
        return "";
    }
}
