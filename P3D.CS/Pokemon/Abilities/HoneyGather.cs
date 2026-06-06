using P3D.Items;

namespace P3D.Abilities;

public class HoneyGather : Ability
{
    public HoneyGather() : base(118, "Honey Gather", "The Pokémon may gather Honey from somewhere.") { }

    public static void GatherHoney()
    {
        foreach (Pokemon p in Core.Player.Pokemons)
        {
            if (p.Ability != null && p.Ability.Name.ToLower() == "honey gather" && p.IsEgg == false)
            {
                if (p.Item == null)
                {
                    int chance = (int)(Math.Ceiling((double)(p.Level / 10)) * 5);
                    if (Core.Random.Next(0, 100) < chance)
                    {
                        p.Item = Item.GetItemByID(253.ToString());
                    }
                }
            }
        }
    }

}
