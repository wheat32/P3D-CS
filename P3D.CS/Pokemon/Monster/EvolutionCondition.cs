namespace P3D;

// TODO Phase 12: full EvolutionCondition port
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

    public enum ConditionTypes { Level, Item, HoldItem, Move, Pokemon, Friendship, Time, Other }

    public class Condition
    {
        public ConditionTypes ConditionType;
        public String Argument = String.Empty;
    }

    public List<Condition> Conditions = [];
    public EvolutionTrigger Trigger;
    public String Evolution = String.Empty;

    public static String EvolutionNumber(Pokemon pokemon, EvolutionTrigger trigger, String argument)
    {
        return String.Empty;
    }

    public static EvolutionCondition GetEvolutionCondition(Pokemon pokemon, EvolutionTrigger trigger, String argument)
    {
        return new EvolutionCondition();
    }
}
