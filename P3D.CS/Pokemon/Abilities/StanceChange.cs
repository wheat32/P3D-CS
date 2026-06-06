namespace P3D.Abilities;

public class StanceChange : Ability
{
    public StanceChange() : base(176, "Stance Change", "The Pokémon changes form depending on how it battles.") { }

    public override void SwitchOut(Pokemon parentPokemon)
    {
        parentPokemon.AdditionalData = "";
        parentPokemon.ReloadDefinitions();
    }

    public override void EndBattle(Pokemon parentPokemon)
    {
        parentPokemon.AdditionalData = "";
        parentPokemon.ReloadDefinitions();
    }

}
