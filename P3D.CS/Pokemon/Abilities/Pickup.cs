using P3D.Items;

namespace P3D.Abilities;

public class Pickup : Ability
{
    public Pickup() : base(53, "Pickup", "The Pokémon may pick up items.") { }

    public static void TryPickup()
    {
        foreach (Pokemon p in Core.Player.Pokemons)
        {
            if (p.Ability != null && p.Ability.Name.ToLower() == "pickup" && p.IsEgg == false)
            {
                if (p.Item == null)
                {
                    int chance = Core.Random.Next(0, 100);
                    List<int> itemList = [];
                    if (chance < 30)
                    {
                        itemList = Get30(p);
                    }
                    else if (chance >= 30 && chance < 40)
                    {
                        itemList = Get10(p);
                    }
                    else if (chance >= 40 && chance < 44)
                    {
                        itemList = Get4(p);
                    }
                    else if (chance == 44)
                    {
                        itemList = Get1(p);
                    }
                    if (itemList.Count > 0)
                    {
                        p.Item = Item.GetItemByID(itemList[Core.Random.Next(0, itemList.Count)].ToString());
                    }
                }
            }
        }
    }

    private static int GetLevelStep(Pokemon p) => (int)Math.Ceiling(p.Level / 10.0);

    // TODO Phase 12: populate item tables from VB source
    private static List<int> Get30(Pokemon p) => [];
    private static List<int> Get10(Pokemon p) => [];
    private static List<int> Get4(Pokemon p) => [];
    private static List<int> Get1(Pokemon p) => [];
}
