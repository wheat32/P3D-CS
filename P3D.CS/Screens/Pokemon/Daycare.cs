using System.Linq;
using P3D;
using P3D.BattleSystem;
using P3D.Items;

namespace P3D;

public static class Daycare
{
    public static Pokemon? ProduceEgg(int daycareID)
    {
        Pokemon? parent1 = null;
        Pokemon? parent2 = null;
        int eggID = 0;

        foreach (String line in Core.Player.DaycareData.SplitAtNewline())
        {
            if (line.StartsWith(daycareID.ToString() + "|0|") == true)
            {
                String data = line.Remove(0, line.IndexOf("{"));
                parent1 = Pokemon.GetPokemonByData(data);
            }
            else if (line.StartsWith(daycareID.ToString() + "|1|") == true)
            {
                String data = line.Remove(0, line.IndexOf("{"));
                parent2 = Pokemon.GetPokemonByData(data);
            }
            else if (line.StartsWith(daycareID.ToString() + "|Egg|") == true)
            {
                eggID = int.Parse(line.Split('|')[2]);
            }
        }

        int dittoAsParent = 0;
        if (parent1 != null && (parent1.EggGroup1 == Pokemon.EggGroups.Ditto || parent1.EggGroup2 == Pokemon.EggGroups.Ditto))
            dittoAsParent = 1;
        else if (parent2 != null && (parent2.EggGroup1 == Pokemon.EggGroups.Ditto || parent2.EggGroup2 == Pokemon.EggGroups.Ditto))
            dittoAsParent = 2;

        if (parent1 != null && parent2 != null && eggID != 0)
        {
            Pokemon p = Pokemon.GetPokemonByID(eggID);
            String optionalAdditionalData = "xXx";

            if (Screen.Level.RegionalForm.Contains(",") == true)
            {
                foreach (String r in Screen.Level.RegionalForm.Split(','))
                {
                    if (p.RegionalForms.Contains(r.ToLower()) == true)
                        p.AdditionalData = r.ToLower();
                }
            }
            else
            {
                if (p.RegionalForms.ToLower().Contains(Screen.Level.RegionalForm.ToLower()) == true)
                    p.AdditionalData = Screen.Level.RegionalForm.ToLower();
            }

            switch (dittoAsParent)
            {
                case 0:
                    if (parent1.Gender == Pokemon.Genders.Female)
                    {
                        if (parent1.Item != null && parent1.Item.OriginalName.ToLower() == "everstone")
                            p.AdditionalData = parent1.AdditionalData;
                        else if (parent2.Number == parent1.Number && parent2.Item != null && parent2.Item.OriginalName.ToLower() == "everstone")
                            p.AdditionalData = parent2.AdditionalData;
                    }
                    else
                    {
                        if (parent2.Item != null && parent2.Item.OriginalName.ToLower() == "everstone")
                            p.AdditionalData = parent2.AdditionalData;
                        else if (parent1.Number == parent2.Number && parent1.Item != null && parent1.Item.OriginalName.ToLower() == "everstone")
                            p.AdditionalData = parent1.AdditionalData;
                    }
                    break;
                case 1:
                    if (parent2.Item != null && parent2.Item.OriginalName.ToLower() == "everstone")
                        p.AdditionalData = parent2.AdditionalData;
                    break;
                case 2:
                    if (parent1.Item != null && parent1.Item.OriginalName.ToLower() == "everstone")
                        p.AdditionalData = parent1.AdditionalData;
                    break;
            }

            if (p.AdditionalData != String.Empty)
                optionalAdditionalData = p.AdditionalData;

            p.Generate(1, true, optionalAdditionalData);
            p.EggSteps = 1;
            p.SetCatchInfos(Item.GetItemByID("5")!, Localization.GetString("CatchMethod_Obtained", "Obtained at"), "Daycare");
            p.CatchBall = Item.GetItemByID(GetEggPokeballID(new List<Pokemon> { parent1, parent2 }))!;

            p.ReloadDefinitions();
            p.CalculateStats();

            List<Attack> eggMoves = [];

            if (dittoAsParent == 0)
            {
                foreach (Attack m1 in parent1.Attacks)
                {
                    foreach (Attack m2 in parent2.Attacks)
                    {
                        if (m1.ID == m2.ID)
                            eggMoves.Add(Attack.GetAttackByID(m1.ID));
                    }
                }
            }

            int male = -1;
            if (parent1.Gender == Pokemon.Genders.Male) male = 0;
            if (parent2.Gender == Pokemon.Genders.Male) male = 1;
            if (male > -1)
            {
                Pokemon cParent = male == 0 ? parent1 : parent2;
                foreach (List<Attack> aList in p.AttackLearns.Values)
                {
                    foreach (Attack thmMove in aList)
                    {
                        foreach (Attack m1 in cParent.Attacks)
                        {
                            if (m1.ID == thmMove.ID)
                                eggMoves.Add(Attack.GetAttackByID(m1.ID));
                        }
                    }
                }
            }

            male = -1;
            if (parent1.Gender == Pokemon.Genders.Male) male = 0;
            if (parent2.Gender == Pokemon.Genders.Male) male = 1;
            if (male > -1)
            {
                foreach (int breedMove in p.EggMoves)
                {
                    foreach (Attack m1 in parent1.Attacks)
                    {
                        if (m1.ID == breedMove)
                        {
                            GameJolt.Emblem.AchieveEmblem("eggsplosion");
                            eggMoves.Add(Attack.GetAttackByID(m1.ID));
                        }
                    }
                    foreach (Attack m1 in parent2.Attacks)
                    {
                        if (m1.ID == breedMove)
                        {
                            GameJolt.Emblem.AchieveEmblem("eggsplosion");
                            eggMoves.Add(Attack.GetAttackByID(m1.ID));
                        }
                    }
                }
            }

            if ((parent1.Item != null && parent1.Item.Name.ToLower() == "light ball") ||
                (parent2.Item != null && parent2.Item.Name.ToLower() == "light ball"))
                eggMoves.Add(Attack.GetAttackByID(344));

            List<Attack> learnMoves = [];
            if (eggMoves.Count <= 4)
            {
                learnMoves.AddRange(eggMoves);
            }
            else
            {
                for (int i = eggMoves.Count - 4; i <= eggMoves.Count - 1; i++)
                    learnMoves.Add(eggMoves[i]);
            }

            while (p.Attacks.Count + learnMoves.Count > 4)
                p.Attacks.RemoveAt(0);

            foreach (Attack learnMove in learnMoves)
            {
                bool hasAttack = false;
                foreach (Attack m in p.Attacks)
                {
                    if (m.ID == learnMove.ID) { hasAttack = true; break; }
                }
                if (hasAttack == false)
                    p.Attacks.Add(learnMove);
            }

            List<String> iv1 = [];
            List<String> iv2 = [];
            String evStat1 = String.Empty;
            String evStat2 = String.Empty;
            bool dKnot = false;

            String[] evItems = ["power weight", "power bracer", "power belt", "power lens", "power band", "power anklet", "destiny knot"];

            if (parent1.Item != null && evItems.Contains(parent1.Item.OriginalName.ToLower()) == true)
            {
                switch (parent1.Item.OriginalName.ToLower())
                {
                    case "power weight": evStat1 = "HP"; break;
                    case "power bracer": evStat1 = "Attack"; break;
                    case "power belt": evStat1 = "Defense"; break;
                    case "power lens": evStat1 = "Special Attack"; break;
                    case "power band": evStat1 = "Special Defense"; break;
                    case "power anklet": evStat1 = "Speed"; break;
                    case "destiny knot": dKnot = true; break;
                }
            }
            if (parent2.Item != null && evItems.Contains(parent2.Item.OriginalName.ToLower()) == true)
            {
                switch (parent2.Item.OriginalName.ToLower())
                {
                    case "power weight": evStat2 = "HP"; break;
                    case "power bracer": evStat2 = "Attack"; break;
                    case "power belt": evStat2 = "Defense"; break;
                    case "power lens": evStat2 = "Special Attack"; break;
                    case "power band": evStat2 = "Special Defense"; break;
                    case "power anklet": evStat2 = "Speed"; break;
                    case "destiny knot": dKnot = true; break;
                }
            }

            if (evStat1 != String.Empty && evStat2 == String.Empty) iv1.Add(evStat1);
            else if (evStat1 == String.Empty && evStat2 != String.Empty) iv2.Add(evStat2);
            else if (evStat1 != String.Empty && evStat2 != String.Empty)
            {
                if (Core.Random.Next(0, 2) == 0) iv1.Add(evStat1);
                else iv2.Add(evStat2);
            }

            int inheritIV = dKnot == true ? 5 : 3;

            while (iv1.Count + iv2.Count < inheritIV)
            {
                String newStat = String.Empty;
                while (newStat == String.Empty || iv1.Contains(newStat) == true || iv2.Contains(newStat) == true)
                {
                    switch (Core.Random.Next(0, 6))
                    {
                        case 0: newStat = "Attack"; break;
                        case 1: newStat = "Defense"; break;
                        case 2: newStat = "Special Attack"; break;
                        case 3: newStat = "Special Defense"; break;
                        case 4: newStat = "Speed"; break;
                        case 5: newStat = "HP"; break;
                    }
                }
                if (Core.Random.Next(0, 2) == 0) iv1.Add(newStat);
                else iv2.Add(newStat);
            }

            foreach (String iv in iv1)
            {
                switch (iv)
                {
                    case "HP": p.IVHP = parent1.IVHP; break;
                    case "Attack": p.IVAttack = parent1.IVAttack; break;
                    case "Defense": p.IVDefense = parent1.IVDefense; break;
                    case "Special Attack": p.IVSpAttack = parent1.IVSpAttack; break;
                    case "Special Defense": p.IVSpDefense = parent1.IVSpDefense; break;
                    case "Speed": p.IVSpeed = parent1.IVSpeed; break;
                }
            }
            foreach (String iv in iv2)
            {
                switch (iv)
                {
                    case "HP": p.IVHP = parent2.IVHP; break;
                    case "Attack": p.IVAttack = parent2.IVAttack; break;
                    case "Defense": p.IVDefense = parent2.IVDefense; break;
                    case "Special Attack": p.IVSpAttack = parent2.IVSpAttack; break;
                    case "Special Defense": p.IVSpDefense = parent2.IVSpDefense; break;
                    case "Speed": p.IVSpeed = parent2.IVSpeed; break;
                }
            }

            bool eStone1 = parent1.Item != null && parent1.Item.OriginalName.ToLower() == "everstone";
            bool eStone2 = parent2.Item != null && parent2.Item.OriginalName.ToLower() == "everstone";

            if (eStone1 == true && eStone2 == false) p.Nature = parent1.Nature;
            else if (eStone1 == false && eStone2 == true) p.Nature = parent2.Nature;
            else if (eStone1 == true && eStone2 == true)
            {
                if (Core.Random.Next(0, 2) == 0) p.Nature = parent1.Nature;
                else p.Nature = parent2.Nature;
            }

            if (dittoAsParent == 0)
            {
                Pokemon female = parent2.Gender == Pokemon.Genders.Female ? parent2 : parent1;
                if (Core.Random.Next(0, 100) < 80)
                {
                    if (p.NewAbilities.Contains(female.Ability) == true)
                    {
                        p.Ability = female.Ability;
                        for (int a = 0; a <= p.NewAbilities.Count - 1; a++)
                        {
                            if (p.Ability.ID == p.NewAbilities[a].ID)
                            {
                                p.AbilitySlot = a == 0 ? "A" : "B";
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int a = 0; a <= female.NewAbilities.Count - 1; a++)
                        {
                            if (female.Ability.ID == female.NewAbilities[a].ID)
                            {
                                p.Ability = p.NewAbilities[a];
                                p.AbilitySlot = a == 0 ? "A" : "B";
                                break;
                            }
                        }
                    }
                }
            }

            if (dittoAsParent != 0)
            {
                Pokemon haParent = dittoAsParent == 1 ? parent2 : parent1;
                if (haParent.IsUsingHiddenAbility == true && p.HasHiddenAbility == true && Core.Random.Next(0, 100) < 60)
                {
                    p.Ability = p.HiddenAbility;
                    p.AbilitySlot = "H";
                }
            }
            else
            {
                Pokemon female = parent2.Gender == Pokemon.Genders.Female ? parent2 : parent1;
                if (female.IsUsingHiddenAbility == true && p.HasHiddenAbility == true && Core.Random.Next(0, 100) < 80)
                {
                    p.Ability = p.HiddenAbility;
                    p.AbilitySlot = "H";
                }
            }

            bool shiny1 = parent1.IsShiny;
            bool shiny2 = parent2.IsShiny;

            List<int> chances = [1, Pokemon.MasterShinyRate];
            if (shiny1 == true && shiny2 == true) chances = [12, Pokemon.MasterShinyRate];
            else if (shiny1 == true || shiny2 == true) chances = [6, Pokemon.MasterShinyRate];

            p.IsShiny = Core.Random.Next(0, chances[1]) < chances[0];

            if (p.Item != null) p.Item = null;
            if (p.HP != p.MaxHP) p.HP = p.MaxHP;

            return p;
        }

        return null;
    }

    public static int CanBreed(List<Pokemon> pokemon, bool multiplier = true)
    {
        int chance = 0;

        if (pokemon.Count == 2)
        {
            Pokemon p1 = pokemon[0];
            Pokemon p2 = pokemon[1];

            if (p1.CanBreed == false || p2.CanBreed == false) return 0;

            if (p1.EggGroup1 == Pokemon.EggGroups.Undiscovered || p1.EggGroup2 == Pokemon.EggGroups.Undiscovered ||
                p2.EggGroup1 == Pokemon.EggGroups.Undiscovered || p2.EggGroup2 == Pokemon.EggGroups.Undiscovered)
                return 0;

            if ((p1.EggGroup1 == Pokemon.EggGroups.Ditto || p1.EggGroup2 == Pokemon.EggGroups.Ditto) &&
                (p2.EggGroup1 == Pokemon.EggGroups.Ditto || p2.EggGroup2 == Pokemon.EggGroups.Ditto))
                return 0;

            if ((p2.EggGroup1 == Pokemon.EggGroups.Ditto || p2.EggGroup2 == Pokemon.EggGroups.Ditto) &&
                (p1.EggGroup1 == Pokemon.EggGroups.Ditto || p1.EggGroup2 == Pokemon.EggGroups.Ditto))
                return 0;

            if (p1.IsGenderless == true)
            {
                if (p2.EggGroup1 == Pokemon.EggGroups.Ditto || p2.EggGroup2 == Pokemon.EggGroups.Ditto)
                    chance = -1;
                else if (p1.EggGroup1 == Pokemon.EggGroups.Ditto || p1.EggGroup2 == Pokemon.EggGroups.Ditto)
                    chance = -1;
            }
            else if (p2.IsGenderless == true)
            {
                if (p1.EggGroup1 == Pokemon.EggGroups.Ditto || p1.EggGroup2 == Pokemon.EggGroups.Ditto)
                    chance = -1;
                else if (p2.EggGroup1 == Pokemon.EggGroups.Ditto || p2.EggGroup2 == Pokemon.EggGroups.Ditto)
                    chance = -1;
            }
            else if (p1.IsGenderless == false && p2.IsGenderless == false)
            {
                if (p1.Gender != p2.Gender)
                {
                    if (p1.EggGroup1 == Pokemon.EggGroups.Ditto || p2.EggGroup1 == Pokemon.EggGroups.Ditto ||
                        p1.EggGroup2 == Pokemon.EggGroups.Ditto || p2.EggGroup2 == Pokemon.EggGroups.Ditto)
                    {
                        chance = -1;
                    }
                    else
                    {
                        if ((p1.EggGroup1 == p2.EggGroup1 && p1.EggGroup1 != Pokemon.EggGroups.None) ||
                            (p1.EggGroup2 == p2.EggGroup1 && p1.EggGroup2 != Pokemon.EggGroups.None) ||
                            (p1.EggGroup1 == p2.EggGroup2 && p1.EggGroup1 != Pokemon.EggGroups.None) ||
                            (p1.EggGroup2 == p2.EggGroup2 && p1.EggGroup2 != Pokemon.EggGroups.None))
                            chance = -1;
                    }
                }
            }

            if (chance == -1)
            {
                if (p1.Number == p2.Number && p1.OT != p2.OT) chance = 70;
                if (p1.Number == p2.Number && p1.OT == p2.OT) chance = 50;
                if (p1.Number != p2.Number && p1.OT != p2.OT) chance = 50;
                if (p1.Number != p2.Number && p1.OT == p2.OT) chance = 20;
            }
        }

        if (chance > 0 && multiplier == true)
        {
            if (Core.Player.Inventory.GetItemAmount("241") > 0)
                chance = (int)(chance * 1.3F);
        }

        return chance;
    }

    public static int CanBreed(int daycareID, bool multiplier = true)
    {
        List<Pokemon> l = [];
        foreach (String line in Core.Player.DaycareData.SplitAtNewline())
        {
            if (line.StartsWith(daycareID.ToString() + "|") == true)
            {
                String data = line.Remove(0, line.IndexOf("{"));
                l.Add(Pokemon.GetPokemonByData(data));
            }
        }
        return CanBreed(l, multiplier);
    }

    public static int CanBreed(Dictionary<int, Pokemon> pokemon, bool multiplier = true)
    {
        List<Pokemon> l = [];
        for (int i = 0; i <= pokemon.Count - 1; i++)
            l.Add(pokemon.Values.ElementAt(i));
        return CanBreed(l, multiplier);
    }

    private static String GetEggPokemonID(Dictionary<int, Pokemon> pokemon)
    {
        if (pokemon.Count == 2)
        {
            Pokemon p1 = pokemon.Values.ElementAt(0);
            Pokemon p2 = pokemon.Values.ElementAt(1);

            if (p1.EggGroup1 == Pokemon.EggGroups.Ditto || p1.EggGroup2 == Pokemon.EggGroups.Ditto)
                return PokemonForms.GetPokemonDataFileName(p2.Number, p2.AdditionalData);
            if (p2.EggGroup1 == Pokemon.EggGroups.Ditto || p2.EggGroup2 == Pokemon.EggGroups.Ditto)
                return PokemonForms.GetPokemonDataFileName(p1.Number, p1.AdditionalData);

            if (p1.Gender == Pokemon.Genders.Female)
                return PokemonForms.GetPokemonDataFileName(p1.Number, p1.AdditionalData);
            if (p2.Gender == Pokemon.Genders.Female)
                return PokemonForms.GetPokemonDataFileName(p2.Number, p2.AdditionalData);
        }
        return "0";
    }

    private static String GetEggPokeballID(List<Pokemon> pokemon)
    {
        String ballID = "5";

        if (pokemon.Count == 2)
        {
            Pokemon p1 = pokemon[0];
            Pokemon p2 = pokemon[1];

            String catchBallID1 = p1.CatchBall.IsGameModeItem == true ? p1.CatchBall.gmID : p1.CatchBall.ID.ToString();
            String catchBallID2 = p2.CatchBall.IsGameModeItem == true ? p2.CatchBall.gmID : p2.CatchBall.ID.ToString();

            if (p1.EggGroup1 == Pokemon.EggGroups.Ditto || p1.EggGroup2 == Pokemon.EggGroups.Ditto)
            {
                if (p2.Gender == Pokemon.Genders.Female) ballID = catchBallID2;
            }
            if (p2.EggGroup1 == Pokemon.EggGroups.Ditto || p2.EggGroup2 == Pokemon.EggGroups.Ditto)
            {
                if (p1.Gender == Pokemon.Genders.Female) ballID = catchBallID1;
            }
            if (p1.EggGroup1 != Pokemon.EggGroups.Ditto && p1.EggGroup2 != Pokemon.EggGroups.Ditto &&
                p2.EggGroup1 != Pokemon.EggGroups.Ditto && p2.EggGroup2 != Pokemon.EggGroups.Ditto)
            {
                if (p1.Gender == Pokemon.Genders.Female) ballID = catchBallID1;
                if (p2.Gender == Pokemon.Genders.Female) ballID = catchBallID2;
            }

            if (ballID == "1" || ballID == "45")
                ballID = "5";
        }

        return ballID;
    }

    public static void ObtainEgg()
    {
        String[] data = Core.Player.DaycareData.SplitAtNewline();
        List<int> ids = [];

        foreach (String line in data)
        {
            if (line != String.Empty && line.Contains("|") == true)
            {
                int newID = int.Parse(line.GetSplit(0, "|"));
                if (ids.Contains(newID) == false)
                    ids.Add(newID);
            }
        }

        Logger.Debug("Daycare circle complete!");

        foreach (int daycareID in ids)
        {
            Logger.Debug("Daycare ID: " + daycareID);

            Dictionary<int, Pokemon> pokemonDict = [];
            bool hasEgg = false;
            int eggID = 0;

            foreach (String line in data)
            {
                if (line.StartsWith(daycareID + "|") == true)
                {
                    if (line.GetSplit(1, "|") == "Egg")
                    {
                        hasEgg = true;
                        eggID = int.Parse(line.GetSplit(2, "|"));
                    }
                    else
                    {
                        int placeID = int.Parse(line.GetSplit(1, "|"));
                        String pokemonData = line.GetSplit(4, "|");

                        if (pokemonDict.ContainsKey(placeID) == false)
                            pokemonDict.Add(placeID, Pokemon.GetPokemonByData(pokemonData));
                    }
                }
            }

            Logger.Debug("Pokémon count: " + pokemonDict.Count);
            Logger.Debug("Has Egg: " + hasEgg.ToString());

            if (hasEgg == false)
            {
                int breedChance = CanBreed(pokemonDict);
                if (breedChance > 0)
                {
                    Logger.Debug("Breed chance: " + breedChance);
                    if (Core.Random.Next(0, 100) < breedChance)
                    {
                        String dexID = GetEggPokemonID(pokemonDict);
                        int parentID = int.Parse(dexID.GetSplit(0, "_"));
                        String parentAD = String.Empty;
                        if (dexID.Contains("_") == true)
                            parentAD = dexID.GetSplit(1, "_");

                        String newEggID = Pokemon.GetPokemonByID(parentID, parentAD).EggPokemon;
                        String s = daycareID.ToString() + "|Egg|" + newEggID;

                        Logger.Debug("Egg created!" + Environment.NewLine + "EggID: " + newEggID);
                        TriggerCall(daycareID);

                        String oldData = Core.Player.DaycareData;
                        if (oldData != String.Empty) oldData += Environment.NewLine;
                        oldData += s;
                        Core.Player.DaycareData = oldData;
                    }
                }
                else
                {
                    Logger.Debug("Pokémon in Daycare " + daycareID + " cannot breed.");
                }
            }
        }
    }

    public static void TriggerCall(int daycareID)
    {
        if (ActionScript.IsRegistered("daycare_callid_" + daycareID.ToString()) == true)
        {
            Object[] c = ActionScript.GetRegisterValue("daycare_callid_" + daycareID.ToString());
            if (c[0] != null && c[1] != null)
            {
                String callID = (String)c[0];
                GameJolt.PokegearScreen.CallID(callID, true, false);
            }
            else
            {
                Logger.Debug("Cannot initialize call for Daycare ID " + daycareID.ToString() + ".");
            }
        }
        else
        {
            Logger.Debug("Cannot initialize call for Daycare ID " + daycareID.ToString() + ".");
        }
    }

    public static void EggCircle()
    {
        Core.Player.PlayerTemp.DayCareCycle = 256;
        ObtainEgg();
    }
}
