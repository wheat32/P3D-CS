namespace P3D;

// TODO Phase 3: full Ability port
public class Ability
{
    public int ID;
    public String Name = String.Empty;
    public String Description = String.Empty;

    public Ability() { }

    public Ability(int id, String name, String description)
    {
        ID = id;
        Name = name;
        Description = description;
    }

    public static Ability? GetAbilityByID(int id) => null;

    public virtual void Activate(Pokemon user) { }
    public virtual void SwitchOut(Pokemon parentPokemon) { }
    public virtual void EndBattle(Pokemon parentPokemon) { }
}
