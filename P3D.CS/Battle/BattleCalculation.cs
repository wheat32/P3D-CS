using P3D;
using P3D.Items;

namespace P3D.BattleSystem;

public static class BattleCalculation
{
    // -----------------------------------------------------------------------
    // FieldEffectTurns
    // Returns the number of turns a field effect lasts, accounting for items.
    // -----------------------------------------------------------------------
    public static int FieldEffectTurns(BattleScreen battleScreen, bool own, String moveName = "")
    {
        int turns = 5;
        Pokemon p = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        String ability = p.Ability.Name.ToLower();
        if (p.Item != null && battleScreen.FieldEffects.CanUseItem(own) == true)
        {
            switch (p.Item.OriginalName.ToLower())
            {
                case "damp rock":
                    if (ability.Equals("drizzle") || moveName.Equals("rain dance"))
                    {
                        turns = 8;
                    }
                    break;
                case "heat rock":
                    if (ability.Equals("drought") || moveName.Equals("sunny day"))
                    {
                        turns = 8;
                    }
                    break;
                case "smooth rock":
                    if (ability.Equals("sand stream") || moveName.Equals("sandstorm"))
                    {
                        turns = 8;
                    }
                    break;
                case "icy rock":
                    if (ability.Equals("snow warning") || moveName.Equals("hail"))
                    {
                        turns = 8;
                    }
                    break;
                case "light clay":
                    if (moveName.Equals("light screen") || moveName.Equals("reflect"))
                    {
                        turns = 8;
                    }
                    break;
                case "terrain extender":
                    if (moveName.Equals("electric terrain") || moveName.Equals("grassy terrain") ||
                        moveName.Equals("misty terrain") || moveName.Equals("psychic terrain"))
                    {
                        turns = 8;
                    }
                    else if (ability.Equals("electric surge") || ability.Equals("grassy surge") ||
                             ability.Equals("misty surge") || ability.Equals("psychic surge"))
                    {
                        turns = 8;
                    }
                    break;
            }
        }
        return turns;
    }

    // -----------------------------------------------------------------------
    // MovesFirst
    // Returns true if the player's Pokemon acts first this turn (speed only).
    // -----------------------------------------------------------------------
    public static bool MovesFirst(BattleScreen battleScreen)
    {
        int ownSpeed = DetermineBattleSpeed(true, battleScreen);
        int oppSpeed = DetermineBattleSpeed(false, battleScreen);
        if (ownSpeed > oppSpeed)
            return true;
        if (ownSpeed < oppSpeed)
            return false;
        return Core.Random.Next(0, 2) == 0;
    }

    // -----------------------------------------------------------------------
    // AttackFirst
    // Returns true if the own attack resolves before the opponent's.
    // Accounts for priority, Quick Claw, Lagging Tail, Full Incense, Stall,
    // Trick Room and base speed comparison.
    // -----------------------------------------------------------------------
    public static bool AttackFirst(Attack ownAttack, Attack oppAttack, BattleScreen battleScreen)
    {
        Pokemon ownPokemon = battleScreen.SelfPokemon!;
        Pokemon oppPokemon = battleScreen.OpponentPokemon!;

        int ownPriority = ownAttack.priority;
        int oppPriority = oppAttack.priority;

        if ("prankster".Equals(ownPokemon.Ability.Name.ToLower()))
        {
            if (ownAttack.category == Attack.Categories.Status)
            {
                ownPriority += 1;
            }
        }
        if ("prankster".Equals(oppPokemon.Ability.Name.ToLower()))
        {
            if (oppAttack.category == Attack.Categories.Status)
            {
                oppPriority += 1;
            }
        }

        if ("gale wings".Equals(ownPokemon.Ability.Name.ToLower()) &&
            ownAttack.type.Type == Element.Types.Flying &&
            ownPokemon.HP == ownPokemon.MaxHP)
        {
            ownPriority += 1;
        }
        if ("gale wings".Equals(oppPokemon.Ability.Name.ToLower()) &&
            oppAttack.type.Type == Element.Types.Flying &&
            oppPokemon.HP == oppPokemon.MaxHP)
        {
            oppPriority += 1;
        }

        if (battleScreen.FieldEffects.CustapBerry.Self > 0)
        {
            ownPriority += 1;
            battleScreen.FieldEffects.CustapBerry.Self = 0;
        }
        if (battleScreen.FieldEffects.CustapBerry.Opponent > 0)
        {
            oppPriority += 1;
            battleScreen.FieldEffects.CustapBerry.Opponent = 0;
        }

        if (ownPriority > oppPriority)
            return true;
        if (oppPriority > ownPriority)
            return false;

        // Same priority — check Quick Claw
        int claw = 0;
        if (ownPokemon.Item != null &&
            battleScreen.FieldEffects.CanUseItem(true) == true &&
            battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
        {
            if (ownPokemon.Item.IsGameModeItem == false && ownPokemon.Item.ID == 73)
            {
                if (Core.Random.Next(0, 100) < 20)
                {
                    claw += 1;
                }
            }
        }
        if (oppPokemon.Item != null &&
            battleScreen.FieldEffects.CanUseItem(false) == true &&
            battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
        {
            if (oppPokemon.Item.IsGameModeItem == false && oppPokemon.Item.ID == 73)
            {
                if (Core.Random.Next(0, 100) < 20)
                {
                    claw += 10;
                }
            }
        }
        switch (claw)
        {
            case 1:
                return true;
            case 10:
                return false;
            case 11:
                return Core.Random.Next(0, 2) == 0;
        }

        // Lagging Tail
        int tail = 0;
        if (ownPokemon.Item != null &&
            battleScreen.FieldEffects.CanUseItem(true) == true &&
            battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
        {
            if ("Lagging Tail".Equals(ownPokemon.Item.Name))
            {
                tail += 1;
            }
        }
        if (oppPokemon.Item != null &&
            battleScreen.FieldEffects.CanUseItem(false) == true &&
            battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
        {
            if ("Lagging Tail".Equals(oppPokemon.Item.Name))
            {
                tail += 10;
            }
        }
        switch (tail)
        {
            case 1:
                return false;
            case 10:
                return true;
            case 11:
                return Core.Random.Next(0, 2) == 0;
        }

        // Full Incense
        int full = 0;
        if (ownPokemon.Item != null &&
            battleScreen.FieldEffects.CanUseItem(true) == true &&
            battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
        {
            if ("Full Incense".Equals(ownPokemon.Item.Name))
            {
                full += 1;
            }
        }
        if (oppPokemon.Item != null &&
            battleScreen.FieldEffects.CanUseItem(false) == true &&
            battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
        {
            if ("Full Incense".Equals(oppPokemon.Item.Name))
            {
                full += 10;
            }
        }
        switch (full)
        {
            case 1:
                return false;
            case 10:
                return true;
            case 11:
                return Core.Random.Next(0, 2) == 0;
        }

        // Stall ability
        if ("stall".Equals(ownPokemon.Ability.Name.ToLower()) &&
            "stall".Equals(oppPokemon.Ability.Name.ToLower()) == false)
        {
            return false;
        }
        if ("stall".Equals(oppPokemon.Ability.Name.ToLower()) &&
            "stall".Equals(ownPokemon.Ability.Name.ToLower()) == false)
        {
            return true;
        }

        // Speed comparison with Trick Room
        int ownSpeed = DetermineBattleSpeed(true, battleScreen);
        int oppSpeed = DetermineBattleSpeed(false, battleScreen);
        bool first;
        if (ownSpeed > oppSpeed)
        {
            first = true;
        }
        else if (ownSpeed < oppSpeed)
        {
            first = false;
        }
        else
        {
            first = Core.Random.Next(0, 2) == 0;
        }

        if (battleScreen.FieldEffects.TrickRoom > 0)
        {
            first = (first == false);
        }
        return first;
    }

    // -----------------------------------------------------------------------
    // DetermineBattleSpeed
    // Returns the effective speed for battle order calculations.
    // -----------------------------------------------------------------------
    public static int DetermineBattleSpeed(bool own, BattleScreen battleScreen)
    {
        Pokemon p = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        int speed = (int)(p.Speed * GetMultiplierFromStat(p.statSpeed));

        if (own == true)
        {
            if (battleScreen.IsPVPBattle == false)
            {
                if (Core.Player.Badges.Contains(3) == true)
                {
                    speed = (int)(speed + (speed * (1.0 / 8)));
                }
            }
        }

        if (p.Status == Pokemon.StatusProblems.Paralyzed &&
            "quick feet".Equals(p.Ability.Name.ToLower()) == false)
        {
            speed = (int)(speed / 2);
        }

        if (p.Item != null)
        {
            if ("Choice Scarf".Equals(p.Item.Name))
            {
                speed = (int)(speed * 1.5F);
            }

            String[] slowDownItems =
            [
                "iron ball", "macho brace", "power bracer", "power belt",
                "power lens", "power band", "power anklet", "power weight"
            ];
            if (slowDownItems.Contains(p.Item.OriginalName.ToLower()) == true &&
                battleScreen.FieldEffects.CanUseItem(own) == true)
            {
                speed = (int)(speed / 2);
            }

            if (own == true)
            {
                if (battleScreen.FieldEffects.TailWind.Self > 0)
                {
                    speed *= 2;
                }
            }
            else
            {
                if (battleScreen.FieldEffects.TailWind.Opponent > 0)
                {
                    speed *= 2;
                }
            }

            if (p.Number == 132)
            {
                if (battleScreen.FieldEffects.CanUseItem(own) == true)
                {
                    if ("Quick Powder".Equals(p.Item.Name))
                    {
                        speed *= 2;
                    }
                }
            }
        }

        switch (p.Ability.Name.ToLower())
        {
            case "swift swim":
                if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain)
                {
                    speed *= 2;
                }
                break;
            case "chlorophyll":
                if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny)
                {
                    speed *= 2;
                }
                break;
            case "sand rush":
                if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sandstorm)
                {
                    speed *= 2;
                }
                break;
            case "slush rush":
                if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
                {
                    speed *= 2;
                }
                break;
            case "surge surfer":
                if (battleScreen.FieldEffects.ElectricTerrain > 0)
                {
                    speed *= 2;
                }
                break;
        }

        int grassPledge = own ? battleScreen.FieldEffects.GrassPledge.Opponent
                               : battleScreen.FieldEffects.GrassPledge.Self;
        if (grassPledge > 0)
        {
            speed = (int)(speed / 2);
        }

        if ("quick feet".Equals(p.Ability.Name.ToLower()))
        {
            if (p.Status == Pokemon.StatusProblems.Paralyzed ||
                p.Status == Pokemon.StatusProblems.Burn ||
                p.Status == Pokemon.StatusProblems.Poison ||
                p.Status == Pokemon.StatusProblems.Sleep ||
                p.Status == Pokemon.StatusProblems.Freeze)
            {
                speed = (int)(speed * 1.5F);
            }
        }

        if ("slow start".Equals(p.Ability.Name.ToLower()))
        {
            int pokemonTurns = own ? battleScreen.FieldEffects.PokemonTurns.Self
                                   : battleScreen.FieldEffects.PokemonTurns.Opponent;
            if (pokemonTurns < 5)
            {
                speed = (int)(speed / 2);
            }
        }

        return speed.Clamp(1, 999);
    }

    // -----------------------------------------------------------------------
    // DetermineBattleAttack
    // Returns the effective Attack stat for damage calculations.
    // -----------------------------------------------------------------------
    public static int DetermineBattleAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        int atkStat = (int)(p.Attack * GetMultiplierFromStat(p.statAttack));

        if (own == true)
        {
            if (battleScreen.IsPVPBattle == false)
            {
                if (Core.Player.Badges.Contains(1) == true)
                {
                    atkStat = (int)(atkStat + (atkStat * (1.0 / 8)));
                }
            }
        }

        if (p.Status == Pokemon.StatusProblems.Burn &&
            "guts".Equals(p.Ability.Name.ToLower()) == false)
        {
            atkStat = (int)(atkStat / 2);
        }

        if (p.Item != null)
        {
            if ("Choice Band".Equals(p.Item.Name))
            {
                atkStat = (int)(atkStat * 1.5F);
            }
            if (p.Number == 25)
            {
                if (battleScreen.FieldEffects.CanUseItem(own) == true)
                {
                    if ("Light Ball".Equals(p.Item.Name))
                    {
                        atkStat *= 2;
                    }
                }
            }
            if (p.Number == 104 || p.Number == 105)
            {
                if (battleScreen.FieldEffects.CanUseItem(own) == true)
                {
                    if ("Thick Club".Equals(p.Item.Name))
                    {
                        atkStat *= 2;
                    }
                }
            }
        }

        switch (p.Ability.Name.ToLower())
        {
            case "huge power":
                atkStat *= 2;
                break;
            case "pure power":
                atkStat *= 2;
                break;
            case "defeatist":
                if ((float)p.HP / p.MaxHP <= 0.5F)
                {
                    atkStat = (int)(atkStat / 2);
                }
                break;
            case "hustle":
                atkStat = (int)(atkStat * 1.5F);
                break;
            case "flower gift":
                if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny)
                {
                    atkStat = (int)(atkStat * 1.5F);
                }
                break;
        }

        if ("guts".Equals(p.Ability.Name.ToLower()))
        {
            if (p.Status == Pokemon.StatusProblems.Paralyzed ||
                p.Status == Pokemon.StatusProblems.Burn ||
                p.Status == Pokemon.StatusProblems.Poison ||
                p.Status == Pokemon.StatusProblems.Sleep ||
                p.Status == Pokemon.StatusProblems.Freeze)
            {
                atkStat = (int)(atkStat * 1.5F);
            }
        }

        if ("slow start".Equals(p.Ability.Name.ToLower()))
        {
            int pokemonTurns = own ? battleScreen.FieldEffects.PokemonTurns.Self
                                   : battleScreen.FieldEffects.PokemonTurns.Opponent;
            if (pokemonTurns < 5)
            {
                atkStat = (int)(atkStat / 2);
            }
        }

        return atkStat.Clamp(1, 999);
    }

    // -----------------------------------------------------------------------
    // ObedienceCheck
    // Returns: 0=obey; 1=sleep talk/snore; 2=use other move; 3=fall asleep;
    //          4-8=ignore (various messages).
    // -----------------------------------------------------------------------
    public static int ObedienceCheck(Attack usedAttack, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon!;

        if (p.OT.Equals(Core.Player.OT))
            return 0;

        if (battleScreen.IsPVPBattle == true)
            return 0;

        int badgeLevel = Badge.GetLevelCap();

        if (badgeLevel > -1 && p.Level > badgeLevel && p.OT.Equals(Core.Player.OT) == false)
        {
            int r = Core.Random.Next(0, 256);
            int a = (int)((p.Level + badgeLevel * r) / 256);
            if (a < badgeLevel)
                return 0;

            battleScreen.FieldEffects.RageCounter.Self = 0;

            if ("snore".Equals(usedAttack.Name.ToLower()) || "sleep talk".Equals(usedAttack.Name.ToLower()))
                return 1;

            r = Core.Random.Next(0, 256);
            int b = (int)((p.Level + badgeLevel) * r / 256);
            if (b < badgeLevel)
                return 2;

            int c = p.Level - badgeLevel;
            r = Core.Random.Next(0, 256);

            int uproar = battleScreen.FieldEffects.Uproar.Self;

            if ("insomnia".Equals(p.Ability.Name.ToLower()) == false &&
                "vital spirit".Equals(p.Ability.Name.ToLower()) == false &&
                uproar == 0)
            {
                if (r < c)
                    return 3;
            }
            else
            {
                if (r - c < c)
                    return 4;
                else
                    return 4 + Core.Random.Next(1, 5);
            }
        }
        return 0;
    }

    // -----------------------------------------------------------------------
    // AccuracyCheck
    // Returns true if the attack hits.
    // -----------------------------------------------------------------------
    public static bool AccuracyCheck(Attack usedAttack, bool own, BattleScreen battleScreen)
    {
        Pokemon p  = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        Pokemon op = own ? battleScreen.OpponentPokemon! : battleScreen.SelfPokemon!;

        if (usedAttack.Accuracy <= 0)
            return true;

        if ("no guard".Equals(p.Ability.Name.ToLower()) ||
            "no guard".Equals(op.Ability.Name.ToLower()))
        {
            return true;
        }

        if (usedAttack.GetUseAccEvasion(own, battleScreen) == false)
            return true;

        float result = 1.0F;

        int init = usedAttack.GetAccuracy(own, battleScreen);
        if ("wonder skin".Equals(op.Ability.Name.ToLower()) &&
            usedAttack.category == Attack.Categories.Status &&
            usedAttack.GetAccuracy(own, battleScreen) > 0 &&
            battleScreen.FieldEffects.CanUseAbility((own == false), battleScreen) == true)
        {
            init = 50;
        }

        if (init < 0)
        {
            init = 1;
        }

        int evasion  = op.evasion;
        int accuracy = p.accuracy;

        if (usedAttack.useOpponentEvasion == false)
        {
            evasion = 0;
        }

        int acc = (accuracy - evasion).Clamp(-6, 6);
        float accM = GetMultiplierFromAccEvasion(acc);
        result = init * accM;

        if (op.Item != null && battleScreen.FieldEffects.CanUseItem((own == false)) == true)
        {
            if ("bright powder".Equals(op.Item.OriginalName.ToLower()) ||
                "lax incense".Equals(op.Item.OriginalName.ToLower()))
            {
                result *= 0.9F;
            }
        }

        if (p.Item != null && battleScreen.FieldEffects.CanUseItem(own) == true)
        {
            switch (p.Item.OriginalName.ToLower())
            {
                case "wide lens":
                    result *= 1.1F;
                    break;
                case "zoom lens":
                {
                    int ownTurns = own ? battleScreen.FieldEffects.TurnCounts.Self
                                       : battleScreen.FieldEffects.TurnCounts.Opponent;
                    int oppTurns = own ? battleScreen.FieldEffects.TurnCounts.Opponent
                                       : battleScreen.FieldEffects.TurnCounts.Self;
                    if (ownTurns < oppTurns)
                    {
                        result *= 1.2F;
                    }
                    break;
                }
            }
        }

        switch (p.Ability.Name.ToLower())
        {
            case "compound eyes":
                result *= 1.3F;
                break;
            case "victory star":
                result *= 1.1F;
                break;
            case "hustle":
                if (usedAttack.category == Attack.Categories.Physical)
                {
                    result *= 0.8F;
                }
                break;
        }

        switch (op.Ability.Name.ToLower())
        {
            case "sand veil":
                if (battleScreen.FieldEffects.CanUseAbility((own == false), battleScreen) == true)
                {
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sandstorm)
                    {
                        result *= 0.8F;
                    }
                }
                break;
            case "snow cloak":
                if (battleScreen.FieldEffects.CanUseAbility((own == false), battleScreen) == true)
                {
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
                    {
                        result *= 0.8F;
                    }
                }
                break;
            case "tangled feet":
                if (battleScreen.FieldEffects.CanUseAbility((own == false), battleScreen) == true)
                {
                    if (op.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true)
                    {
                        result *= 0.5F;
                    }
                }
                break;
        }

        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Foggy)
        {
            result *= 0.6F;
        }
        if (battleScreen.FieldEffects.Gravity > 0)
        {
            result *= (float)(10.0 / 6);
        }

        int f = ((int)result).Clamp(0, 100);
        int r = Core.Random.Next(0, 100);
        return r < f;
    }

    // -----------------------------------------------------------------------
    // IsCriticalHit
    // Returns true if the move is a critical hit.
    // -----------------------------------------------------------------------
    public static bool IsCriticalHit(Attack usedAttack, bool own, BattleScreen battleScreen)
    {
        int c = 0;
        Pokemon p  = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        Pokemon op = own ? battleScreen.OpponentPokemon! : battleScreen.SelfPokemon!;

        if (usedAttack.criticalChance < 1)
            return false;

        if ("battle armor".Equals(op.Ability.Name.ToLower()) ||
            "shell armor".Equals(op.Ability.Name.ToLower()))
        {
            if (battleScreen.FieldEffects.CanUseAbility((own == false), battleScreen) == true)
                return false;
        }

        int luckyChant = own ? battleScreen.FieldEffects.LuckyChant.Opponent
                              : battleScreen.FieldEffects.LuckyChant.Self;
        if (luckyChant > 0)
            return false;

        if ("super luck".Equals(p.Ability.Name.ToLower()))
        {
            c += 1;
        }

        int focusEnergy = own ? battleScreen.FieldEffects.FocusEnergy.Self
                               : battleScreen.FieldEffects.FocusEnergy.Opponent;
        if (focusEnergy > 0)
        {
            c += 2;
        }

        if (usedAttack.criticalChance > 1)
        {
            c += 1;
        }

        int lansat = own ? battleScreen.FieldEffects.LansatBerry.Self
                         : battleScreen.FieldEffects.LansatBerry.Opponent;
        if (lansat > 0)
        {
            c += 2;
        }

        if (p.Item != null && battleScreen.FieldEffects.CanUseItem(own) == true)
        {
            switch (p.Item.OriginalName.ToLower())
            {
                case "lucky punch":
                    if (p.Number == 113)
                    {
                        c += 2;
                    }
                    break;
                case "leek":
                    if (p.Number == 83)
                    {
                        c += 2;
                    }
                    break;
                case "scope lens":
                    c += 1;
                    break;
                case "razor claw":
                    c += 1;
                    break;
            }
        }

        int chance;
        switch (c)
        {
            case 0:
                chance = 16;
                break;
            case 1:
                chance = 8;
                break;
            case 2:
                chance = 4;
                break;
            case 3:
                chance = 3;
                break;
            default:
                chance = 2;
                break;
        }

        if (Core.Random.Next(0, chance) == 0)
            return true;
        if (usedAttack.ID == 524) // Frost Breath — always critical
            return true;
        if (usedAttack.ID == 480) // Storm Throw — always critical
            return true;

        return false;
    }

    // -----------------------------------------------------------------------
    // CanRun
    // Returns true if the battler can escape.
    // -----------------------------------------------------------------------
    public static bool CanRun(bool own, BattleScreen battleScreen, bool isEscapeMove = false)
    {
        if (battleScreen.BattleMode == BattleScreen.BattleModes.Safari)
            return true;

        if (BattleScreen.CanAlwaysRun == true)
            return true;

        Pokemon p  = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        Pokemon op = own ? battleScreen.OpponentPokemon! : battleScreen.SelfPokemon!;

        if (p.Type1.Type == Element.Types.Ghost || p.Type2.Type == Element.Types.Ghost)
            return true;

        if ("run away".Equals(p.Ability.Name.ToLower()))
            return true;

        if (p.Item != null)
        {
            if ("smoke ball".Equals(p.Item.OriginalName.ToLower()) &&
                battleScreen.FieldEffects.CanUseItem(own) == true &&
                battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                return true;
            }
        }

        if ("shadow tag".Equals(op.Ability.Name.ToLower()) &&
            "shadow tag".Equals(p.Ability.Name.ToLower()) == false &&
            op.HP > 0)
        {
            return false;
        }
        if ("arena trap".Equals(op.Ability.Name.ToLower()) &&
            op.HP > 0 &&
            battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            return false;
        }
        if ("magnet pull".Equals(op.Ability.Name.ToLower()) && op.HP > 0)
        {
            if (p.Type1.Type == Element.Types.Steel || p.Type2.Type == Element.Types.Steel)
                return false;
        }

        FieldEffects fe = battleScreen.FieldEffects;
        if (own == true)
        {
            if (fe.Wrap.Self > 0 || fe.Bind.Self > 0 || fe.Clamp.Self > 0 ||
                fe.FireSpin.Self > 0 || fe.MagmaStorm.Self > 0 || fe.SandTomb.Self > 0 ||
                fe.Whirlpool.Self > 0 || fe.Infestation.Self > 0)
            {
                return false;
            }
        }
        else
        {
            if (fe.Wrap.Opponent > 0 || fe.Bind.Opponent > 0 || fe.Clamp.Opponent > 0 ||
                fe.FireSpin.Opponent > 0 || fe.MagmaStorm.Opponent > 0 || fe.SandTomb.Opponent > 0 ||
                fe.Whirlpool.Opponent > 0 || fe.Infestation.Opponent > 0)
            {
                return false;
            }
        }

        int ingrain = own ? fe.Ingrain.Self : fe.Ingrain.Opponent;
        if (ingrain > 0)
            return false;

        if (isEscapeMove == true)
            return true;

        if (p.Speed > op.Speed)
            return true;

        int a = p.Speed;
        int b = op.Speed;
        if (b == 0)
        {
            b = 1;
        }
        int runTries = fe.RunTries;
        int x = ((int)(a * 128 / b) + (30 * runTries)) % 256;
        int rVal = Core.Random.Next(0, 256);
        if (rVal < x)
            return true;

        fe.RunTries += 1;
        return false;
    }

    // -----------------------------------------------------------------------
    // CalculateEffectiveness (full version)
    // -----------------------------------------------------------------------
    public static float CalculateEffectiveness(Attack move, BattleScreen battleScreen,
                                                Pokemon p, Pokemon op, bool own)
    {
        float type1 = ReverseTypeEffectiveness(
            Element.GetElementMultiplier(move.GetAttackType(own, battleScreen), op.Type1));
        float type2 = ReverseTypeEffectiveness(
            Element.GetElementMultiplier(move.GetAttackType(own, battleScreen), op.Type2));
        float effectiveness = type1 * type2;

        // Freeze Dry — super effective vs Water regardless of type chart
        if (move.ID == 573)
        {
            if (op.Type1.Type == Element.Types.Water || op.Type2.Type == Element.Types.Water)
            {
                effectiveness *= 4;
            }
        }

        // Flying Press — Flying + Fighting dual-type
        if (move.ID == 9999)
        {
            if (op.Type1.Type == Element.Types.Fighting || op.Type2.Type == Element.Types.Fighting)
            {
                effectiveness *= 2;
            }
            if (op.Type1.Type == Element.Types.Grass || op.Type2.Type == Element.Types.Grass)
            {
                effectiveness *= 2;
            }
            if (op.Type1.Type == Element.Types.Bug || op.Type2.Type == Element.Types.Bug)
            {
                effectiveness *= 2;
            }
            if (op.Type1.Type == Element.Types.Rock || op.Type2.Type == Element.Types.Rock)
            {
                effectiveness /= 2;
            }
            if (op.Type1.Type == Element.Types.Steel || op.Type2.Type == Element.Types.Steel)
            {
                effectiveness /= 2;
            }
            if (op.Type1.Type == Element.Types.Electric || op.Type2.Type == Element.Types.Electric)
            {
                effectiveness /= 2;
            }
        }

        // Sheer Cold — no effect vs Ice types
        if (move.ID == 329)
        {
            if (op.IsType(Element.Types.Ice))
            {
                effectiveness = 0.0F;
            }
        }

        bool digHit = false;
        bool airHit = false;
        bool regHit = false;

        FieldEffects fe = battleScreen.FieldEffects;

        if (move.GetAttackType(own, battleScreen).Type == Element.Types.Ground &&
            fe.IsGrounded((own == false), battleScreen) == false)
        {
            bool targetDig = false;
            bool targetAir = false;

            if (own == true)
            {
                if (fe.DigCounter.Opponent > 0)
                {
                    targetDig = true;
                }
                if (fe.FlyCounter.Opponent > 0 || fe.BounceCounter.Opponent > 0 ||
                    fe.SkyDropCounter.Opponent > 0)
                {
                    targetAir = true;
                }
            }
            else
            {
                if (fe.DigCounter.Self > 0)
                {
                    targetDig = true;
                }
                if (fe.FlyCounter.Self > 0 || fe.BounceCounter.Self > 0 ||
                    fe.SkyDropCounter.Self > 0)
                {
                    targetAir = true;
                }
            }

            switch (move.ID)
            {
                case 89:   // Earthquake
                case 90:   // Fissure
                case 222:  // Magnitude
                    effectiveness = 0.0F;
                    if (targetDig == true)
                    {
                        digHit = true;
                    }
                    break;
                case 614:  // Thousand Arrows
                    airHit = true;
                    if (targetAir == true)
                    {
                        if (own == true)
                        {
                            fe.FlyCounter.Opponent = 0;
                            fe.BounceCounter.Opponent = 0;
                        }
                        else
                        {
                            fe.FlyCounter.Self = 0;
                            fe.BounceCounter.Self = 0;
                        }
                    }
                    break;
                default:
                    effectiveness = 0.0F;
                    break;
            }
        }
        else if (move.GetAttackType(own, battleScreen).Type == Element.Types.Ground &&
                 fe.IsGrounded((own == false), battleScreen) == true)
        {
            regHit = true;
        }

        if (digHit == true || airHit == true || regHit == true)
        {
            effectiveness = 1.0F;
            if (op.IsType(Element.Types.Electric)) { effectiveness *= 2; }
            if (op.IsType(Element.Types.Fire))     { effectiveness *= 2; }
            if (op.IsType(Element.Types.Poison))   { effectiveness *= 2; }
            if (op.IsType(Element.Types.Rock))     { effectiveness *= 2; }
            if (op.IsType(Element.Types.Steel))    { effectiveness *= 2; }
            if (op.IsType(Element.Types.Bug))      { effectiveness /= 2; }
            if (op.IsType(Element.Types.Grass))    { effectiveness /= 2; }
        }

        // Ring Target — cancels one immunity
        if (op.Item != null)
        {
            if ("ring target".Equals(op.Item.OriginalName.ToLower()) &&
                fe.CanUseItem((own == false)) == true &&
                fe.CanUseOwnItem((own == false), battleScreen) == true)
            {
                if (type1 == 0)
                {
                    effectiveness = type2;
                }
                if (type2 == 0)
                {
                    effectiveness = type1;
                }
                if (effectiveness == 0)
                {
                    effectiveness = 1.0F;
                }
            }
        }

        // Moves that bypass immunity (ImmunityAffected = false)
        if (move.immunityAffected == false && effectiveness == 0)
        {
            if (type1 == 0.0F)
            {
                effectiveness = type2;
            }
            if (type2 == 0.0F)
            {
                effectiveness = type1;
            }
            if (effectiveness == 0.0F)
            {
                effectiveness = 1.0F;
            }
        }

        // Foresight / Odor Sleuth / Scrappy — Normal and Fighting hit Ghost
        if (op.IsType(Element.Types.Ghost) == true)
        {
            bool canHitGhost = false;
            int foresight = own ? fe.Foresight.Opponent : fe.Foresight.Self;
            int odorSleuth = own ? fe.OdorSleuth.Opponent : fe.OdorSleuth.Self;
            if (foresight > 0 || odorSleuth > 0)
            {
                canHitGhost = true;
            }
            if (fe.CanUseAbility(own, battleScreen) == true &&
                "scrappy".Equals(p.Ability.Name.ToLower()))
            {
                canHitGhost = true;
            }
            if (canHitGhost == true)
            {
                if (move.type.Type == Element.Types.Normal ||
                    move.type.Type == Element.Types.Fighting)
                {
                    if (type1 == 0.0F)
                    {
                        effectiveness = type2;
                    }
                    if (type2 == 0.0F)
                    {
                        effectiveness = type1;
                    }
                    if (effectiveness == 0.0F)
                    {
                        effectiveness = 1.0F;
                    }
                }
            }
        }

        // Miracle Eye — Psychic hits Dark
        if (op.IsType(Element.Types.Dark) == true)
        {
            int miracleEye = own ? fe.MiracleEye.Opponent : fe.MiracleEye.Self;
            if (miracleEye > 0)
            {
                if (move.type.Type == Element.Types.Psychic)
                {
                    if (type1 == 0.0F)
                    {
                        effectiveness = type2;
                    }
                    if (type2 == 0.0F)
                    {
                        effectiveness = type1;
                    }
                    if (effectiveness == 0.0F)
                    {
                        effectiveness = 1.0F;
                    }
                }
            }
        }

        // Tar Shot — doubles Fire effectiveness on target
        if (move.GetAttackType(own, battleScreen).Type == Element.Types.Fire)
        {
            bool tarShot = own ? fe.TarShot.Opponent : fe.TarShot.Self;
            if (tarShot == true)
            {
                effectiveness *= 2;
            }
        }

        if (move.useEffectiveness == false && effectiveness != 0.0F)
            return 1.0F;

        return effectiveness;
    }

    // -----------------------------------------------------------------------
    // CalculateEffectiveness (wrapper — determines attacker/defender from own)
    // -----------------------------------------------------------------------
    public static float CalculateEffectiveness(bool own, Attack move, BattleScreen battleScreen)
    {
        Pokemon p  = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        Pokemon op = own ? battleScreen.OpponentPokemon! : battleScreen.SelfPokemon!;
        return CalculateEffectiveness(move, battleScreen, p, op, own);
    }

    // -----------------------------------------------------------------------
    // GainExp
    // Calculates experience gained by a Pokemon after defeating an opponent.
    // -----------------------------------------------------------------------
    public static int GainExp(Pokemon p, BattleScreen battleScreen,
                               List<int> pokemonList, int pokeIndex)
    {
        Pokemon op = battleScreen.OpponentPokemon!;

        double a = 1.0;
        if (battleScreen.IsTrainerBattle == true)
        {
            a = 1.5;
        }

        double b = op.BaseExperience;

        double t = 1.0;
        if (p.Item != null && p.Item.IsGameModeItem == true)
        {
            GameModeItem gmi = (GameModeItem)p.Item;
            if (gmi.gmExpMultiplier != -1.0 && gmi.gmOverrideTradeExp == true)
            {
                t = gmi.gmExpMultiplier;
            }
            else
            {
                if (p.OT.Equals(Core.Player.OT) == false)
                {
                    t = 1.5;
                }
            }
        }
        else
        {
            if (p.OT.Equals(Core.Player.OT) == false)
            {
                t = 1.5;
            }
        }

        double gm = 1.0;
        if (p.Item != null && p.Item.IsGameModeItem == true)
        {
            GameModeItem gmi = (GameModeItem)p.Item;
            if (gmi.gmExpMultiplier != -1.0 && gmi.gmOverrideTradeExp == false)
            {
                gm = gmi.gmExpMultiplier;
            }
        }

        double e = 1.0;
        if (p.Item != null)
        {
            if ("lucky egg".Equals(p.Item.OriginalName.ToLower()))
            {
                e = 1.5;
            }
        }

        double l  = op.Level;
        double lp = p.Level;
        double s  = pokemonList.Count;

        float expAllMultiplier = 1;
        if (Core.Player.Inventory.GetItemAmount(658.ToString()) > 0 &&
            Core.Player.EnableExpAll == true)
        {
            s = 1.0;
            if (battleScreen.ParticipatedPokemon.Contains(pokeIndex) == false)
            {
                expAllMultiplier = 0.5F;
            }
        }
        else
        {
            int expShares = 0;
            foreach (Pokemon po in Core.Player.Pokemons)
            {
                if (po.Item != null)
                {
                    if ("exp. share".Equals(po.Item.OriginalName.ToLower()))
                    {
                        expShares += 1;
                    }
                }
            }
            if (expShares > 0)
            {
                if (p.Item != null && "exp. share".Equals(p.Item.OriginalName.ToLower()))
                {
                    s = 2.0;
                }
                else
                {
                    s = (pokemonList.Count * 2.0) * expShares;
                }
            }
        }

        int exp = (int)(
            (((a * b * l) / (5 * s)) *
             (Math.Pow((2 * l + 10), 2.5) / Math.Pow((l + lp + 10), 2.5)) + 1)
            * t * e * gm * expAllMultiplier);

        if (exp < 2)
        {
            exp = 2;
        }

        foreach (MysteryEvent mysteryEvent in MysteryEventScreen.ActivatedMysteryEvents)
        {
            if (mysteryEvent.EventType == MysteryEventScreen.EventTypes.EXPMultiplier)
            {
                exp = (int)(exp * float.Parse(
                    mysteryEvent.Value.Replace(".", GameController.DecSeparator)));
            }
        }

        if (Core.Player.Inventory.GetItemAmount(656.ToString()) > 0) // Exp. Charm
        {
            exp = (int)(exp * 1.5F);
        }

        return exp;
    }

    // -----------------------------------------------------------------------
    // SafariRound
    // Determines the wild Pokemon's action in a Safari Zone battle.
    // -----------------------------------------------------------------------
    public static BattleRoundConst SafariRound(BattleScreen battleScreen)
    {
        if (battleScreen.PokemonSafariStatus > 0)
        {
            battleScreen.PokemonSafariStatus -= 1;
        }
        else if (battleScreen.PokemonSafariStatus < 0)
        {
            battleScreen.PokemonSafariStatus += 1;
        }

        bool flee = false;

        float x = battleScreen.OpponentPokemon!.Speed % 256;
        x *= 2;
        if (x > 255)
        {
            flee = true;
        }

        if (flee == false)
        {
            if (battleScreen.PokemonSafariStatus < 0)
            {
                x *= 2;
            }
            else if (battleScreen.PokemonSafariStatus > 0)
            {
                x /= 4;
            }
        }

        int r = Core.Random.Next(0, 256);
        if (r < x)
        {
            flee = true;
        }

        if (flee == true && CanSwitch(battleScreen, false) == true)
        {
            return new BattleRoundConst()
            {
                StepType = BattleRoundConst.StepTypes.Flee,
                Argument = battleScreen.OpponentPokemon.GetDisplayName() + " fled!"
            };
        }

        String statusMessage;
        if (battleScreen.PokemonSafariStatus < 0)
        {
            statusMessage = battleScreen.OpponentPokemon.GetDisplayName() + " is angry!";
        }
        else if (battleScreen.PokemonSafariStatus > 0)
        {
            statusMessage = battleScreen.OpponentPokemon.GetDisplayName() + " is eating!";
        }
        else
        {
            statusMessage = battleScreen.OpponentPokemon.GetDisplayName() + " is watching carefully!";
        }
        return new BattleRoundConst()
        {
            StepType = BattleRoundConst.StepTypes.Text,
            Argument = statusMessage
        };
    }

    // -----------------------------------------------------------------------
    // CanSwitch
    // Returns true if the specified side can switch out their Pokemon.
    // -----------------------------------------------------------------------
    public static bool CanSwitch(BattleScreen battleScreen, bool own)
    {
        FieldEffects fe = battleScreen.FieldEffects;

        if (own == true)
        {
            if (battleScreen.SelfPokemon!.Status == Pokemon.StatusProblems.Fainted ||
                battleScreen.SelfPokemon.HP <= 0)
            {
                return true;
            }
            if (battleScreen.SelfPokemon.Type1.Type == Element.Types.Ghost ||
                battleScreen.SelfPokemon.Type2.Type == Element.Types.Ghost)
            {
                return true;
            }
            if (battleScreen.SelfPokemon.Item != null)
            {
                if ("shed shell".Equals(battleScreen.SelfPokemon.Item.OriginalName.ToLower()) &&
                    fe.CanUseItem(true) == true &&
                    fe.CanUseOwnItem(true, battleScreen) == true)
                {
                    return true;
                }
            }
            if (battleScreen.IsRemoteBattle == true && battleScreen.IsPVPBattle == true &&
                battleScreen.IsHost == false)
            {
                if (fe.ClientCanSwitch == false)
                    return false;
            }
            if ("shadow tag".Equals(battleScreen.OpponentPokemon!.Ability.Name.ToLower()) &&
                "shadow tag".Equals(battleScreen.SelfPokemon.Ability.Name.ToLower()) == false &&
                battleScreen.OpponentPokemon.HP > 0)
            {
                return false;
            }
            if (fe.TrappedCounter.Self > 0)
                return false;
            if (battleScreen.OppFaint == false)
            {
                if ("arena trap".Equals(battleScreen.OpponentPokemon!.Ability.Name.ToLower()) &&
                    battleScreen.OpponentPokemon.HP > 0 &&
                    fe.IsGrounded(true, battleScreen) == true)
                {
                    return false;
                }
            }
            if ("magnet pull".Equals(battleScreen.OpponentPokemon!.Ability.Name.ToLower()) &&
                battleScreen.SelfPokemon.IsType(Element.Types.Steel) == true &&
                battleScreen.OpponentPokemon.HP > 0)
            {
                return false;
            }
            if (fe.Wrap.Self > 0 || fe.Bind.Self > 0 || fe.Clamp.Self > 0 ||
                fe.FireSpin.Self > 0 || fe.MagmaStorm.Self > 0 || fe.SandTomb.Self > 0 ||
                fe.Whirlpool.Self > 0 || fe.Infestation.Self > 0)
            {
                return false;
            }
            if (fe.Ingrain.Self > 0)
                return false;
        }
        else
        {
            if (battleScreen.OpponentPokemon!.Status == Pokemon.StatusProblems.Fainted ||
                battleScreen.OpponentPokemon.HP <= 0)
            {
                return true;
            }
            if (battleScreen.OpponentPokemon.Type1.Type == Element.Types.Ghost ||
                battleScreen.OpponentPokemon.Type2.Type == Element.Types.Ghost)
            {
                return true;
            }
            if (battleScreen.OpponentPokemon.Item != null)
            {
                if ("shed shell".Equals(battleScreen.OpponentPokemon.Item.OriginalName.ToLower()) &&
                    fe.CanUseItem(false) == true &&
                    fe.CanUseOwnItem(false, battleScreen) == true)
                {
                    return true;
                }
            }
            if ("shadow tag".Equals(battleScreen.SelfPokemon!.Ability.Name.ToLower()) &&
                "shadow tag".Equals(battleScreen.OpponentPokemon.Ability.Name.ToLower()) == false &&
                battleScreen.SelfPokemon.HP > 0)
            {
                return false;
            }
            if (fe.TrappedCounter.Opponent > 0)
                return false;
            if (battleScreen.OwnFaint == false)
            {
                if ("arena trap".Equals(battleScreen.SelfPokemon.Ability.Name.ToLower()) &&
                    battleScreen.SelfPokemon.HP > 0 &&
                    fe.IsGrounded(false, battleScreen) == true)
                {
                    return false;
                }
            }
            if ("magnet pull".Equals(battleScreen.SelfPokemon.Ability.Name.ToLower()) &&
                battleScreen.OpponentPokemon.IsType(Element.Types.Steel) == true &&
                battleScreen.SelfPokemon.HP > 0)
            {
                return false;
            }
            if (fe.Wrap.Opponent > 0 || fe.Bind.Opponent > 0 || fe.Clamp.Opponent > 0 ||
                fe.FireSpin.Opponent > 0 || fe.MagmaStorm.Opponent > 0 || fe.SandTomb.Opponent > 0 ||
                fe.Whirlpool.Opponent > 0 || fe.Infestation.Opponent > 0)
            {
                return false;
            }
            if (fe.Ingrain.Opponent > 0)
                return false;
        }

        return true;
    }

    // -----------------------------------------------------------------------
    // CalculateDamage
    // Core damage formula. Returns final damage dealt.
    // -----------------------------------------------------------------------
    public static int CalculateDamage(Attack attack, bool critical, bool own,
                                       bool targetPokemon, BattleScreen battleScreen,
                                       String extraParam = "",
                                       Attack? typeEffectivenessAttack = null)
    {
        Pokemon p  = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        Pokemon op = own ? battleScreen.OpponentPokemon! : battleScreen.SelfPokemon!;

        int damage = 0;
        int level = p.Level;

        // Base-power multipliers
        float hh = 1.0F; // Helping Hand
        float bp = (float)attack.GetBasePower(own, battleScreen);
        float it = 1.0F; // Type-boosting item
        float chg = 1.0F; // Charge
        float ms = 1.0F; // Mud Sport
        float ws = 1.0F; // Water Sport
        float ua = 1.0F; // User Ability
        float fa = 1.0F; // Foe Ability

        // IT — type-boosting held item
        if (p.Item != null &&
            battleScreen.FieldEffects.CanUseItem(own) == true &&
            battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
        {
            switch (p.Item.OriginalName.ToLower())
            {
                case "muscle band":
                    if (attack.category == Attack.Categories.Physical)
                    {
                        it = 1.1F;
                    }
                    break;
                case "adamant orb":
                    if (p.Number == 483)
                    {
                        if (attack.type.Type == Element.Types.Dragon ||
                            attack.type.Type == Element.Types.Steel)
                        {
                            it = 1.2F;
                        }
                    }
                    break;
                case "lustrous orb":
                    if (p.Number == 484)
                    {
                        if (attack.type.Type == Element.Types.Dragon ||
                            attack.type.Type == Element.Types.Water)
                        {
                            it = 1.2F;
                        }
                    }
                    break;
                case "griseous orb":
                    if (p.Number == 487)
                    {
                        if (attack.type.Type == Element.Types.Dragon ||
                            attack.type.Type == Element.Types.Ghost)
                        {
                            it = 1.2F;
                        }
                    }
                    break;
                case "soul dew":
                    if (p.Number == 380 || p.Number == 381)
                    {
                        if (attack.type.Type == Element.Types.Dragon ||
                            attack.type.Type == Element.Types.Psychic)
                        {
                            it = 1.2F;
                        }
                    }
                    break;
                default:
                    it = 1.0F;
                    break;
            }

            if (p.Item.IsGameModeItem == false)
            {
                // Type-plate and type-boosting items by ID
                switch (p.Item.ID)
                {
                    case 98: case 270:   // Black Belt, Fist Plate
                        if (attack.type.Type == Element.Types.Fighting) { it = 1.2F; }
                        break;
                    case 102: case 268:  // Black Glasses, Dread Plate
                        if (attack.type.Type == Element.Types.Dark) { it = 1.2F; }
                        break;
                    case 138: case 271:  // Charcoal, Flame Plate
                        if (attack.type.Type == Element.Types.Fire) { it = 1.2F; }
                        break;
                    case 144: case 267:  // Dragon Fang, Draco Plate
                        if (attack.type.Type == Element.Types.Dragon) { it = 1.2F; }
                        break;
                    case 125: case 281: case 286:  // Hard Stone, Stone Plate, Rock Incense
                        if (attack.type.Type == Element.Types.Rock) { it = 1.2F; }
                        break;
                    case 108: case 283:  // Magnet, Zap Plate
                        if (attack.type.Type == Element.Types.Electric) { it = 1.2F; }
                        break;
                    case 143: case 274:  // Metal Coat, Iron Plate
                        if (attack.type.Type == Element.Types.Steel) { it = 1.2F; }
                        break;
                    case 117: case 275: case 287:  // Miracle Seed, Meadow Plate, Rose Incense
                        if (attack.type.Type == Element.Types.Grass) { it = 1.2F; }
                        break;
                    case 95: case 145: case 264: case 279: // Mystic Water, Wave/Sea Incense, Splash Plate
                        if (attack.type.Type == Element.Types.Water) { it = 1.2F; }
                        break;
                    case 107: case 272:  // NeverMeltIce, Icicle Plate
                        if (attack.type.Type == Element.Types.Ice) { it = 1.2F; }
                        break;
                    case 104: case 277:  // Pink Bow, Pixie Plate
                        if (attack.type.Type == Element.Types.Fairy) { it = 1.2F; }
                        break;
                    case 81: case 282:   // Poison Barb, Toxic Plate
                        if (attack.type.Type == Element.Types.Poison) { it = 1.2F; }
                        break;
                    case 77: case 278:   // Sharp Beak, Sky Plate
                        if (attack.type.Type == Element.Types.Flying) { it = 1.2F; }
                        break;
                    case 88: case 273:   // Silver Powder, Insect Plate
                        if (attack.type.Type == Element.Types.Bug) { it = 1.2F; }
                        break;
                    case 76: case 269:   // Soft Sand, Earth Plate
                        if (attack.type.Type == Element.Types.Ground) { it = 1.2F; }
                        break;
                    case 113: case 280:  // Spell Tag, Spooky Plate
                        if (attack.type.Type == Element.Types.Ghost) { it = 1.2F; }
                        break;
                    case 96: case 263: case 276: // Twisted Spoon, Odd Incense, Mind Plate
                        if (attack.type.Type == Element.Types.Psychic) { it = 1.2F; }
                        break;
                    case 90:  // Silk Scarf
                        if (attack.type.Type == Element.Types.Normal) { it = 1.2F; }
                        break;
                }

                // Gems — 1.3× one-use boost
                switch (p.Item.ID)
                {
                    case 635:  // Fighting Gem
                        if (attack.type.Type == Element.Types.Fighting)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:fighting gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Fighting Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 644:  // Dark Gem
                        if (attack.type.Type == Element.Types.Dark)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:dark gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Dark Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 630:  // Fire Gem
                        if (attack.type.Type == Element.Types.Fire)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:fire gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Fire Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 643:  // Dragon Gem
                        if (attack.type.Type == Element.Types.Dragon)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:dragon gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Dragon Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 641:  // Rock Gem
                        if (attack.type.Type == Element.Types.Rock)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:rock gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Rock Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 632:  // Electric Gem
                        if (attack.type.Type == Element.Types.Electric)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:electric gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Electric Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 645:  // Steel Gem
                        if (attack.type.Type == Element.Types.Steel)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:steel gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Steel Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 633:  // Grass Gem
                        if (attack.type.Type == Element.Types.Grass)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:grass gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Grass Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 631:  // Water Gem
                        if (attack.type.Type == Element.Types.Water)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:water gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Water Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 634:  // Ice Gem
                        if (attack.type.Type == Element.Types.Ice)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:ice gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Ice Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 647:  // Fairy Gem
                        if (attack.type.Type == Element.Types.Fairy)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:fairy gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Fairy Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 636:  // Poison Gem
                        if (attack.type.Type == Element.Types.Poison)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:poison gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Poison Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 638:  // Flying Gem
                        if (attack.type.Type == Element.Types.Flying)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:flying gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Flying Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 640:  // Bug Gem
                        if (attack.type.Type == Element.Types.Bug)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:bug gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Bug Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 637:  // Ground Gem
                        if (attack.type.Type == Element.Types.Ground)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:ground gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Ground Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 642:  // Ghost Gem
                        if (attack.type.Type == Element.Types.Ghost)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:ghost gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Ghost Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 639:  // Psychic Gem
                        if (attack.type.Type == Element.Types.Psychic)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:psychic gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Psychic Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                    case 646:  // Normal Gem
                        if (attack.type.Type == Element.Types.Normal)
                        {
                            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "-1",
                                    "item:normal gem") == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(
                                    "The Normal Gem boosted " + p.GetDisplayName() +
                                    "'s " + attack.Name + "!"));
                                it = 1.3F;
                            }
                        }
                        break;
                }
            }
        }

        // CHG — Charge move doubling Electric damage
        FieldEffects fe = battleScreen.FieldEffects;
        Attack? lastMove = own ? fe.LastMove.Self : fe.LastMove.Opponent;
        if (lastMove != null)
        {
            if ("charge".Equals(lastMove.Name.ToLower()) &&
                attack.type.Type == Element.Types.Electric)
            {
                chg = 2.0F;
            }
        }

        // MS / WS — Mud Sport and Water Sport
        if (fe.MudSport > 0 && attack.type.Type == Element.Types.Electric)
        {
            ms = 0.5F;
        }
        if (fe.WaterSport > 0 && attack.type.Type == Element.Types.Fire)
        {
            ws = 0.5F;
        }

        // UA — User Ability multiplier
        switch (p.Ability.Name.ToLower())
        {
            case "rivalry":
                if (p.Gender != Pokemon.Genders.Genderless &&
                    op.Gender != Pokemon.Genders.Genderless)
                {
                    ua = p.Gender == op.Gender ? 1.25F : 0.75F;
                }
                break;
            case "reckless":
                if (attack.isRecoilMove == true)
                {
                    ua = 1.2F;
                }
                break;
            case "iron fist":
                if (attack.isPunchingMove == true)
                {
                    ua = 1.2F;
                }
                break;
            case "blaze":
                if (p.HP < (int)Math.Floor((double)p.MaxHP / 3) &&
                    attack.type.Type == Element.Types.Fire)
                {
                    ua = 1.5F;
                }
                break;
            case "overgrow":
                if (p.HP < (int)Math.Floor((double)p.MaxHP / 3) &&
                    attack.type.Type == Element.Types.Grass)
                {
                    ua = 1.5F;
                }
                break;
            case "torrent":
                if (p.HP < (int)Math.Floor((double)p.MaxHP / 3) &&
                    attack.type.Type == Element.Types.Water)
                {
                    ua = 1.5F;
                }
                break;
            case "swarm":
                if (p.HP < (int)Math.Floor((double)p.MaxHP / 3) &&
                    attack.type.Type == Element.Types.Bug)
                {
                    ua = 1.5F;
                }
                break;
            case "technician":
                if (attack.GetBasePower(own, battleScreen) <= 60)
                {
                    ua = 1.5F;
                }
                break;
            case "sheer force":
                if (attack.hasSecondaryEffect == true)
                {
                    ua = 1.3F;
                }
                break;
            case "analytic":
            {
                int ownTurns = fe.TurnCounts.Self;
                int oppTurns = fe.TurnCounts.Opponent;
                bool movingLast = own ? (ownTurns < oppTurns) : (ownTurns > oppTurns);
                if (movingLast == true)
                {
                    ua = 1.3F;
                }
                break;
            }
            case "sand force":
                if (fe.Weather == BattleWeather.WeatherTypes.Sandstorm)
                {
                    if (attack.type.Type == Element.Types.Rock ||
                        attack.type.Type == Element.Types.Ground ||
                        attack.type.Type == Element.Types.Steel)
                    {
                        ua = 1.3F;
                    }
                }
                break;
            case "strong jaw":
                if (attack.isJawMove == true)
                {
                    ua = 1.5F;
                }
                break;
            case "refrigerate":
                if (attack.type.Type == Element.Types.Normal)
                {
                    ua = 1.2F;
                }
                break;
            case "pixilate":
                if (attack.type.Type == Element.Types.Normal)
                {
                    ua = 1.2F;
                }
                break;
            case "normalize":
                if (attack.type.Type == Element.Types.Normal)
                {
                    ua = 1.2F;
                }
                break;
            case "galvanize":
                if (attack.type.Type == Element.Types.Normal)
                {
                    ua = 1.2F;
                }
                break;
            case "aerilate":
                if (attack.type.Type == Element.Types.Normal)
                {
                    ua = 1.2F;
                }
                break;
            case "mega launcher":
                if (attack.isPulseMove == true)
                {
                    ua = 1.5F;
                }
                break;
            case "tough claws":
                if (attack.makesContact == true)
                {
                    ua = 1.3F;
                }
                break;
            case "dark aura":
                if (attack.type.Type == Element.Types.Dark)
                {
                    ua = 1.3F;
                }
                break;
            case "fairy aura":
                if (attack.type.Type == Element.Types.Fairy)
                {
                    ua = 1.3F;
                }
                break;
            case "parental bond":
                if ("parental bond".Equals(extraParam))
                {
                    ua = 0.25F;
                }
                break;
            default:
                ua = 1.0F;
                break;
        }

        // FA — Foe Ability multiplier
        switch (op.Ability.Name.ToLower())
        {
            case "thick fat":
                if (fe.CanUseAbility((own == false), battleScreen) == true)
                {
                    if (attack.type.Type == Element.Types.Fire ||
                        attack.type.Type == Element.Types.Ice)
                    {
                        fa = 0.5F;
                    }
                }
                break;
            case "heatproof":
                if (fe.CanUseAbility(own, battleScreen) == true)
                {
                    if (attack.type.Type == Element.Types.Fire)
                    {
                        fa = 0.5F;
                    }
                }
                break;
            case "dry skin":
                if (fe.CanUseAbility((own == false), battleScreen) == true)
                {
                    if (attack.type.Type == Element.Types.Fire)
                    {
                        fa = 1.25F;
                    }
                }
                break;
            case "multiscale":
                if (op.HP == op.MaxHP)
                {
                    fa = 0.5F;
                }
                break;
            case "dark aura":
                if (attack.type.Type == Element.Types.Dark)
                {
                    fa = 1.3F;
                }
                break;
            case "fairy aura":
                if (attack.type.Type == Element.Types.Fairy)
                {
                    fa = 1.3F;
                }
                break;
            default:
                fa = 1.0F;
                break;
        }

        int basePower = (int)Math.Floor(hh * bp * it * chg * ms * ws * ua * fa);

        // ATK
        float aStat = 1.0F;
        float aSM   = 1.0F;
        float aM    = 1.0F;
        float iM    = 1.0F;

        if (attack.category == Attack.Categories.Physical)
        {
            if (attack.ID == 492) // Foul Play — uses opponent's Attack
            {
                aStat = attack.GetUseAttackStat(op);
                aSM   = GetMultiplierFromStat(op.statAttack);
            }
            else
            {
                aStat = attack.GetUseAttackStat(p);
                aSM   = GetMultiplierFromStat(p.statAttack);
            }

            if (fe.CanUseAbility((own == false), battleScreen) == true)
            {
                if ("unaware".Equals(op.Ability.Name.ToLower()))
                {
                    aSM = 1.0F;
                }
            }

            switch (p.Ability.Name.ToLower())
            {
                case "pure power":
                    aM = 2.0F;
                    break;
                case "huge power":
                    aM = 2.0F;
                    break;
                case "flower gift":
                    if (fe.CanUseAbility(own, battleScreen) == true)
                    {
                        if (fe.Weather == BattleWeather.WeatherTypes.Sunny)
                        {
                            aM = 1.5F;
                        }
                    }
                    break;
                case "guts":
                    if (p.Status == Pokemon.StatusProblems.Paralyzed ||
                        p.Status == Pokemon.StatusProblems.Poison ||
                        p.Status == Pokemon.StatusProblems.Burn ||
                        p.Status == Pokemon.StatusProblems.Sleep ||
                        p.Status == Pokemon.StatusProblems.BadPoison)
                    {
                        aM = 1.5F;
                    }
                    break;
                case "hustle":
                    aM = 1.5F;
                    break;
                case "slow start":
                {
                    int pokemonTurns = own ? fe.PokemonTurns.Self : fe.PokemonTurns.Opponent;
                    if (pokemonTurns < 5)
                    {
                        aM = 0.5F;
                    }
                    break;
                }
                case "toxic boost":
                    if (p.Status == Pokemon.StatusProblems.Poison ||
                        p.Status == Pokemon.StatusProblems.BadPoison)
                    {
                        aM = 1.5F;
                    }
                    break;
                case "defeatist":
                    if (p.HP <= (int)(p.MaxHP / 2))
                    {
                        aM = 0.5F;
                    }
                    break;
                default:
                    aM = 1.0F;
                    break;
            }

            if (p.Item != null &&
                fe.CanUseItem(own) == true &&
                fe.CanUseOwnItem(own, battleScreen) == true)
            {
                switch (p.Item.OriginalName.ToLower())
                {
                    case "choice band":
                        iM = 1.5F;
                        break;
                    case "light ball":
                        if (p.Number == 25)
                        {
                            iM = 2.0F;
                        }
                        break;
                    case "thick club":
                        if (p.Number == 104 || p.Number == 105)
                        {
                            iM = 2.0F;
                        }
                        break;
                    default:
                        iM = 1.0F;
                        break;
                }
            }
        }
        else if (attack.category == Attack.Categories.Special)
        {
            aStat = attack.GetUseAttackStat(p);
            aSM   = GetMultiplierFromStat(p.statSpAttack);

            if ("unaware".Equals(op.Ability.Name.ToLower()))
            {
                aSM = 1.0F;
            }

            switch (p.Ability.Name.ToLower())
            {
                case "plus":
                    aM = 1.0F;
                    break;
                case "minus":
                    aM = 1.0F;
                    break;
                case "solar power":
                    if (fe.Weather == BattleWeather.WeatherTypes.Sunny)
                    {
                        aM = 1.5F;
                    }
                    break;
                case "flare boost":
                    if (op.Status == Pokemon.StatusProblems.Burn)
                    {
                        aM = 1.5F;
                    }
                    break;
                case "defeatist":
                    if (p.HP <= (int)(p.MaxHP / 2))
                    {
                        aM = 0.5F;
                    }
                    break;
                default:
                    aM = 1.0F;
                    break;
            }

            if (p.Item != null &&
                fe.CanUseItem(own) == true &&
                fe.CanUseOwnItem(own, battleScreen) == true)
            {
                switch (p.Item.OriginalName.ToLower())
                {
                    case "choice specs":
                        iM = 1.5F;
                        break;
                    case "light ball":
                        if (p.Number == 25)
                        {
                            iM = 2.0F;
                        }
                        break;
                    case "deepseatooth":
                        if (p.Number == 366)
                        {
                            iM = 2.0F;
                        }
                        break;
                    case "metronome":
                    {
                        Attack? lastAtk = own ? fe.LastMove.Opponent : fe.LastMove.Self;
                        if (lastAtk != null)
                        {
                            if (lastAtk.ID == attack.ID)
                            {
                                if (own == true)
                                {
                                    fe.MetronomeItemCount.Self += 1;
                                    iM = 1.0F + ((float)fe.MetronomeItemCount.Self.Clamp(1, 10) / 10);
                                }
                                else
                                {
                                    fe.MetronomeItemCount.Opponent += 1;
                                    iM = 1.0F + ((float)fe.MetronomeItemCount.Opponent.Clamp(1, 10) / 10);
                                }
                            }
                            else
                            {
                                if (own == true)
                                {
                                    fe.MetronomeItemCount.Self = 0;
                                }
                                else
                                {
                                    fe.MetronomeItemCount.Opponent = 0;
                                }
                                iM = 1.0F;
                            }
                        }
                        else
                        {
                            if (own == true)
                            {
                                fe.MetronomeItemCount.Self = 0;
                            }
                            else
                            {
                                fe.MetronomeItemCount.Opponent = 0;
                            }
                            iM = 1.0F;
                        }
                        break;
                    }
                    default:
                        iM = 1.0F;
                        break;
                }
            }
        }

        // Critical hit drops beneficial attacker stat changes
        if (aSM < 1.0F && critical == true)
        {
            aSM = 1.0F;
        }

        int atk = (int)Math.Floor(aStat * aSM * aM * iM);

        // DEF
        float dStat = 1.0F;
        float dSM   = 1.0F;
        float sX    = 1.0F;  // Self-Destruct / Explosion (unused since Gen 5+)
        float dMod  = 1.0F;

        // Psyshock (473), Psystrike (540), Secret Sword (548) use physical defense
        if (attack.category == Attack.Categories.Physical ||
            attack.ID == 473 || attack.ID == 540 || attack.ID == 548)
        {
            dStat = attack.GetUseDefenseStat(op);
            dSM   = GetMultiplierFromStat(op.statDefense);

            if ("unaware".Equals(p.Ability.Name.ToLower()))
            {
                dSM = 1.0F;
            }

            if ("self-destruct".Equals(attack.Name.ToLower()) ||
                "explosion".Equals(attack.Name.ToLower()))
            {
                sX = 1.0F;
            }

            if (op.Item != null &&
                fe.CanUseItem((own == false)) == true &&
                fe.CanUseOwnItem((own == false), battleScreen) == true)
            {
                switch (op.Item.OriginalName.ToLower())
                {
                    case "metal powder":
                        if (op.Number == 132)
                        {
                            dMod = 1.5F;
                        }
                        break;
                    case "eviolite":
                        if (op.IsFullyEvolved() == false)
                        {
                            dMod = 1.5F;
                        }
                        break;
                }
            }

            if ("marvel scale".Equals(op.Ability.Name.ToLower()))
            {
                if (fe.CanUseAbility((own == false), battleScreen) == true)
                {
                    if (op.Status == Pokemon.StatusProblems.Paralyzed ||
                        op.Status == Pokemon.StatusProblems.Poison ||
                        op.Status == Pokemon.StatusProblems.Burn ||
                        op.Status == Pokemon.StatusProblems.Sleep ||
                        op.Status == Pokemon.StatusProblems.Freeze)
                    {
                        dMod = 1.5F;
                    }
                }
            }
            if ("fur coat".Equals(op.Ability.Name.ToLower()) &&
                fe.CanUseAbility((own == false), battleScreen) == true)
            {
                dMod = 2.0F;
            }
            if (fe.GrassyTerrain > 0 &&
                "grass pelt".Equals(op.Ability.Name.ToLower()) &&
                fe.CanUseAbility((own == false), battleScreen) == true)
            {
                dMod = 1.5F;
            }
        }
        else if (attack.category == Attack.Categories.Special)
        {
            dStat = attack.GetUseDefenseStat(op);
            dSM   = GetMultiplierFromStat(op.statSpDefense);

            if ("unaware".Equals(p.Ability.Name.ToLower()) ||
                attack.useOpponentDefense == false)
            {
                dSM = 1.0F;
            }

            if ("flower gift".Equals(op.Ability.Name.ToLower()))
            {
                if (fe.Weather == BattleWeather.WeatherTypes.Sunny)
                {
                    dMod = 1.5F;
                }
            }

            if (op.Item != null &&
                fe.CanUseItem((own == false)) == true &&
                fe.CanUseOwnItem((own == false), battleScreen) == true)
            {
                switch (op.Item.OriginalName.ToLower())
                {
                    case "metal powder":
                        if (op.Number == 132)
                        {
                            dMod = 1.5F;
                        }
                        break;
                    case "deepseascale":
                        if (op.Number == 366)
                        {
                            dMod = 2.0F;
                        }
                        break;
                    case "assault vest":
                        dMod = 1.5F;
                        break;
                    case "eviolite":
                        if (op.IsFullyEvolved() == false)
                        {
                            dMod = 1.5F;
                        }
                        break;
                }
            }

            if (fe.Weather == BattleWeather.WeatherTypes.Sandstorm)
            {
                if (op.Type1.Type == Element.Types.Rock || op.Type2.Type == Element.Types.Rock)
                {
                    dMod = 1.5F;
                }
            }
        }

        // Critical hit drops beneficial defender stat changes
        if (dSM > 1.0F && critical == true)
        {
            dSM = 1.0F;
        }

        // Sacred Sword (ID 533) ignores defense stat changes
        if (attack.ID == 533)
        {
            dSM = 1.0F;
        }

        int def = (int)Math.Floor(dStat * dSM * dMod * sX);
        if (def <= 0)
        {
            def = 1;
        }

        // Mod1
        float brN = 1.0F; // Burn penalty
        float rL  = 1.0F; // Reflect / Light Screen
        float tVT = 1.0F; // (unused placeholder)
        float sR  = 1.0F; // Sun / Rain / terrain weather
        float fF  = 1.0F; // Flash Fire

        if (attack.category == Attack.Categories.Physical)
        {
            if ("guts".Equals(p.Ability.Name.ToLower()) == false &&
                p.Status == Pokemon.StatusProblems.Burn)
            {
                brN = 0.5F;
            }
        }

        bool critSwap = Core.Random.Next(0, 3) == 0 && critical == true;
        if (critSwap == false)
        {
            if (attack.category == Attack.Categories.Physical)
            {
                if ("infiltrator".Equals(p.Ability.Name.ToLower()) == false)
                {
                    int reflectVal = own ? fe.Reflect.Opponent : fe.Reflect.Self;
                    if (reflectVal > 0)
                    {
                        rL = 0.5F;
                    }
                }
            }
            else if (attack.category == Attack.Categories.Special)
            {
                if ("infiltrator".Equals(p.Ability.Name.ToLower()) == false)
                {
                    int lightScreenVal = own ? fe.LightScreen.Opponent : fe.LightScreen.Self;
                    if (lightScreenVal > 0)
                    {
                        rL = 0.5F;
                    }
                }
            }
        }

        switch (attack.type.Type)
        {
            case Element.Types.Fire:
                if (fe.Weather == BattleWeather.WeatherTypes.Sunny)
                {
                    sR = 1.5F;
                }
                else if (fe.Weather == BattleWeather.WeatherTypes.Rain)
                {
                    sR = 0.5F;
                }
                break;
            case Element.Types.Water:
                if (fe.Weather == BattleWeather.WeatherTypes.Sunny)
                {
                    sR = 0.5F;
                }
                else if (fe.Weather == BattleWeather.WeatherTypes.Rain)
                {
                    sR = 1.5F;
                }
                break;
            case Element.Types.Ice:
                if (fe.Weather == BattleWeather.WeatherTypes.Snow)
                {
                    sR = 1.5F;
                }
                break;
            case Element.Types.Electric:
                if (fe.ElectricTerrain > 0 &&
                    fe.IsGrounded(own, battleScreen) == true)
                {
                    sR = 1.3F;
                }
                break;
            case Element.Types.Grass:
                if (fe.GrassyTerrain > 0 &&
                    fe.IsGrounded(own, battleScreen) == true)
                {
                    sR = 1.3F;
                }
                break;
            case Element.Types.Psychic:
                if (fe.PsychicTerrain > 0 &&
                    fe.IsGrounded(own, battleScreen) == true)
                {
                    sR = 1.3F;
                }
                break;
        }

        // Flash Fire — boosts own Fire moves after absorbing a Fire attack
        if (attack.type.Type == Element.Types.Fire)
        {
            int flashFireVal = own ? fe.FlashFire.Self : fe.FlashFire.Opponent;
            if (flashFireVal == 1)
            {
                fF = 1.5F;
            }
        }

        float mod1 = brN * rL * tVT * sR * fF;

        // CH — critical hit multiplier
        float cH = 1.0F;
        if (critical == true)
        {
            cH = "sniper".Equals(p.Ability.Name.ToLower()) ? 2.25F : 1.5F;
        }

        // Mod2 — Life Orb, Me First
        float mod2 = 1.0F;
        if (p.Item != null &&
            fe.CanUseItem(own) == true &&
            fe.CanUseOwnItem(own, battleScreen) == true)
        {
            if ("life orb".Equals(p.Item.OriginalName.ToLower()))
            {
                mod2 = 1.3F;
            }
        }
        if ("me first".Equals(attack.Name.ToLower()))
        {
            mod2 = 1.5F;
        }

        // R — random roll 85-100
        int rRoll = Core.Random.Next(85, 101);
        if (rRoll == 0)
        {
            rRoll = 1;
        }

        // STAB
        float stab = 1.0F;
        if (attack.canGainSTAB == true)
        {
            if (attack.type.Type == p.Type1.Type || attack.type.Type == p.Type2.Type)
            {
                stab = "adaptability".Equals(p.Ability.Name.ToLower()) ? 2.0F : 1.5F;
            }
        }

        // Mod3 — Solid Rock/Filter, Expert Belt, Tinted Lens, type-reducing berries
        float sRF = 1.0F; // Solid Rock / Filter
        float eB  = 1.0F; // Expert Belt
        float tL  = 1.0F; // Tinted Lens
        float tRB = 1.0F; // Type-Reducing Berry

        float effectiveness = typeEffectivenessAttack != null
            ? CalculateEffectiveness(typeEffectivenessAttack, battleScreen, p, op, own)
            : CalculateEffectiveness(attack, battleScreen, p, op, own);

        if (effectiveness > 1.0F)
        {
            if ("solid rock".Equals(op.Ability.Name.ToLower()) ||
                "filter".Equals(op.Ability.Name.ToLower()))
            {
                if (fe.CanUseAbility((own == false), battleScreen) == true)
                {
                    sRF = 0.75F;
                }
            }

            if (p.Item != null &&
                fe.CanUseItem(own) == true &&
                fe.CanUseOwnItem(own, battleScreen) == true)
            {
                if ("expert belt".Equals(p.Item.OriginalName.ToLower()))
                {
                    eB = 1.2F;
                }
            }

            // Type-reducing berries (opponent's)
            if (op.Item != null)
            {
                if (fe.CanUseItem((own == false)) == true &&
                    fe.CanUseOwnItem((own == false), battleScreen) == true)
                {
                    String opItem = op.Item.OriginalName.ToLower();
                    if ("occa".Equals(opItem) && attack.type.Type == Element.Types.Fire)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Occa Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:occa") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("passho".Equals(opItem) && attack.type.Type == Element.Types.Water)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Passho Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:passho") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("wacan".Equals(opItem) && attack.type.Type == Element.Types.Electric)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Wacan Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:wacan") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("rindo".Equals(opItem) && attack.type.Type == Element.Types.Grass)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Rindo Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:rindo") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("yache".Equals(opItem) && attack.type.Type == Element.Types.Ice)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Yache Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:yache") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("chople".Equals(opItem) && attack.type.Type == Element.Types.Fighting)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Chople Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:chople") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("kebia".Equals(opItem) && attack.type.Type == Element.Types.Poison)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Kebia Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:kebia") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("shuca".Equals(opItem) && attack.type.Type == Element.Types.Ground)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Shuca Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:shuca") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("coba".Equals(opItem) && attack.type.Type == Element.Types.Flying)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Coba Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:coba") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("payapa".Equals(opItem) && attack.type.Type == Element.Types.Psychic)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Payapa Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:payapa") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("tanga".Equals(opItem) && attack.type.Type == Element.Types.Bug)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Tanga Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:tanga") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("charti".Equals(opItem) && attack.type.Type == Element.Types.Rock)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Charti Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:charti") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("kasib".Equals(opItem) && attack.type.Type == Element.Types.Ghost)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Kasib Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:kasib") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("haban".Equals(opItem) && attack.type.Type == Element.Types.Dragon)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Haban Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:haban") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("colbur".Equals(opItem) && attack.type.Type == Element.Types.Dark)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Colbur Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:colbur") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("babiri".Equals(opItem) && attack.type.Type == Element.Types.Steel)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Babiri Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:babiri") == true)
                        { tRB = 0.5F; }
                    }
                    else if ("roseli".Equals(opItem) && attack.type.Type == Element.Types.Fairy)
                    {
                        if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                                "The Roseli Berry weakened the effect of " + attack.Name +
                                " on " + op.GetDisplayName() + "!", "berry:roseli") == true)
                        { tRB = 0.5F; }
                    }
                }
            }
        }

        // Chilan Berry — always reduces Normal damage (not just super effective)
        if (op.Item != null)
        {
            if (fe.CanUseItem((own == false)) == true &&
                fe.CanUseOwnItem((own == false), battleScreen) == true)
            {
                if ("chilan".Equals(op.Item.OriginalName.ToLower()) &&
                    attack.type.Type == Element.Types.Normal)
                {
                    if (battleScreen.Battle.RemoveHeldItem((own == false), (own == false), battleScreen,
                            "The Chilan Berry weakened the effect of " + attack.Name +
                            " on " + op.GetDisplayName() + "!", "berry:chilan") == true)
                    {
                        tRB = 0.5F;
                    }
                }
            }
        }

        // Tinted Lens — doubles damage of not-very-effective moves
        if (effectiveness < 1.0F)
        {
            if ("tinted lens".Equals(p.Ability.Name.ToLower()))
            {
                tL = 2.0F;
            }
        }

        float mod3 = sRF * eB * tL * tRB;

        // Final damage formula
        damage = (int)Math.Floor(
            ((((((level * 2.0 / 5) + 2) * basePower * atk / 50.0) / def) * mod1) + 2)
            * cH * mod2 * rRoll / 100.0 * stab * effectiveness * mod3);

        // Multiscale — attacker's (if own Pokemon has Multiscale at full HP)
        if ("multiscale".Equals(p.Ability.Name.ToLower()) &&
            p.HP == p.MaxHP &&
            fe.CanUseAbility(own, battleScreen) == true)
        {
            damage = (int)(damage / 2);
        }

        // Misty Terrain — halves Dragon moves vs grounded
        if (fe.MistyTerrain > 0 && fe.IsGrounded(own, battleScreen) == true)
        {
            if (attack.type.Type == Element.Types.Dragon)
            {
                damage = (int)(damage / 2);
            }
        }

        // Grassy Terrain — halves Earthquake, Bulldoze, Magnitude vs grounded
        if (fe.GrassyTerrain > 0 && fe.IsGrounded(own, battleScreen) == true)
        {
            if (attack.ID == 89 || attack.ID == 523 || attack.ID == 222)
            {
                damage = (int)(damage / 2);
            }
        }

        // One-hit KO moves
        if (attack.isOneHitKOMove == true)
        {
            damage = op.HP;
        }

        if (damage <= 0)
        {
            damage = 1;
        }

        return damage;
    }

    // -----------------------------------------------------------------------
    // ReverseTypeEffectiveness
    // Inverts effectiveness values for Inverse Battles.
    // -----------------------------------------------------------------------
    public static float ReverseTypeEffectiveness(float effectiveness)
    {
        if (BattleScreen.IsInverseBattle == true)
        {
            if (effectiveness == 0.5F || effectiveness == 0.0F)
                return 2.0F;
            if (effectiveness == 1.0F)
                return 1.0F;
            if (effectiveness == 2.0F)
                return 0.5F;
            return 1.0F;
        }
        return effectiveness;
    }

    // -----------------------------------------------------------------------
    // GetMultiplierFromStat
    // Converts a stat stage (-6 to +6) to its damage multiplier.
    // -----------------------------------------------------------------------
    public static float GetMultiplierFromStat(int statValue)
    {
        switch (statValue)
        {
            case -6: return (float)(2.0 / 8);
            case -5: return (float)(2.0 / 7);
            case -4: return (float)(2.0 / 6);
            case -3: return (float)(2.0 / 5);
            case -2: return (float)(2.0 / 4);
            case -1: return (float)(2.0 / 3);
            case  0: return (float)(2.0 / 2);
            case  1: return (float)(3.0 / 2);
            case  2: return (float)(4.0 / 2);
            case  3: return (float)(5.0 / 2);
            case  4: return (float)(6.0 / 2);
            case  5: return (float)(7.0 / 2);
            case  6: return (float)(8.0 / 2);
            default: return 1.0F;
        }
    }

    // -----------------------------------------------------------------------
    // GetMultiplierFromAccEvasion
    // Converts an accuracy/evasion stage (-6 to +6) to its hit-chance multiplier.
    // -----------------------------------------------------------------------
    public static float GetMultiplierFromAccEvasion(int statValue)
    {
        switch (statValue)
        {
            case -6: return (float)(3.0 / 9);
            case -5: return (float)(3.0 / 8);
            case -4: return (float)(3.0 / 7);
            case -3: return (float)(3.0 / 6);
            case -2: return (float)(3.0 / 5);
            case -1: return (float)(3.0 / 4);
            case  0: return (float)(3.0 / 3);
            case  1: return (float)(4.0 / 3);
            case  2: return (float)(5.0 / 3);
            case  3: return (float)(6.0 / 3);
            case  4: return (float)(7.0 / 3);
            case  5: return (float)(8.0 / 3);
            case  6: return (float)(9.0 / 3);
            default: return 1.0F;
        }
    }
}
