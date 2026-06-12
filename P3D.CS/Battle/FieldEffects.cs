using P3D;
using P3D.Items;

namespace P3D.BattleSystem;

public class FieldEffects
{
    // --- Client-side PvP flag ---
    public bool ClientCanSwitch = true;

    // --- Per-side counters: (Self, Opponent) ---
    public (int Self, int Opponent) AquaRing;
    public (int Self, int Opponent) BanefulBunkerCounter;
    public (int Self, int Opponent) BatonPassIndex = (-1, -1);
    public (int Self, int Opponent) BideCounter;
    public (int Self, int Opponent) BideDamage;
    public (int Self, int Opponent) Bind;
    public (int Self, int Opponent) BounceCounter;
    public (int Self, int Opponent) Charge;
    public (int Self, int Opponent) Clamp;
    public (int Self, int Opponent) ConfusionTurns;
    public (int Self, int Opponent) TempConfusionTurns;
    public (Item? Self, Item? Opponent) ConsumedItem;
    public (int Self, int Opponent) CraftyShieldCounter;
    public (Item? Self, Item? Opponent) CudChewBerry;
    public (int Self, int Opponent) CudChewIndex = (-1, -1);
    public (int Self, int Opponent) CustapBerry;
    public (int Self, int Opponent) Curse;
    public (int Self, int Opponent) DefenseCurl;
    public (bool Self, bool Opponent) DestinyBond;
    public (int Self, int Opponent) DetectCounter;
    public (int Self, int Opponent) DigCounter;
    public (int Self, int Opponent) DiveCounter;
    public (int Self, int Opponent) EchoedVoice;
    public (int Self, int Opponent) Embargo;
    public (int Self, int Opponent) Encore;
    public (Attack? Self, Attack? Opponent) EncoreMove;
    public (int Self, int Opponent) Endure;
    public (int Self, int Opponent) FirePledge;
    public (int Self, int Opponent) FireSpin;
    public (int Self, int Opponent) FlashFire;
    public (int Self, int Opponent) FlyCounter;
    public (int Self, int Opponent) FocusEnergy;
    public (int Self, int Opponent) Foresight;
    public (int Self, int Opponent) FreezeShockCounter;
    public (int Self, int Opponent) FuryCutter;
    public (int Self, int Opponent) FutureSightDamage;
    public (int Self, int Opponent) FutureSightID;
    public (int Self, int Opponent) FutureSightTurns;
    public (bool Self, bool Opponent) GastroAcid;
    public (int Self, int Opponent) GeomancyCounter;
    public (int Self, int Opponent) GrassPledge;
    public (int Self, int Opponent) GuardSpec;
    public (int Self, int Opponent) HealBlock;
    public (bool Self, bool Opponent) HealingWish;
    public (int Self, int Opponent) IceBallCounter;
    public (int Self, int Opponent) IceBurnCounter;
    public (int Self, int Opponent) Imprison;
    public (int Self, int Opponent) Infestation;
    public (int Self, int Opponent) Ingrain;
    public (int Self, int Opponent) KingsShieldCounter;
    public (int Self, int Opponent) LansatBerry;
    public (int Self, int Opponent) LastDamage;
    public (Attack? Self, Attack? Opponent) LastMove;
    public (bool Self, bool Opponent) LastMoveFailed;
    public (int Self, int Opponent) LeechSeed;
    public (int Self, int Opponent) LightScreen;
    public (int Self, int Opponent) LockOn;
    public (int Self, int Opponent) LuckyChant;
    public (int Self, int Opponent) MagicCoat;
    public (int Self, int Opponent) MagmaStorm;
    public (int Self, int Opponent) MagnetRise;
    public (int Self, int Opponent) MatBlockCounter;
    public (bool Self, bool Opponent) MegaEvolved;
    public (int Self, int Opponent) MetronomeItemCount;
    public (int Self, int Opponent) Minimize;
    public (int Self, int Opponent) MiracleEye;
    public (int Self, int Opponent) Mist;
    public (int Self, int Opponent) Nightmare;
    public (int Self, int Opponent) OdorSleuth;
    public (int Self, int Opponent) Outrage;
    public (int Self, int Opponent) PayDayCounter;
    public (int Self, int Opponent) PerishSongCount;
    public (int Self, int Opponent) PetalDance;
    public (int Self, int Opponent) PhantomForceCounter;
    public (int Self, int Opponent) PoisonCounter;
    public (bool Self, bool Opponent) PokemonDamagedLastTurn;
    public (bool Self, bool Opponent) PokemonDamagedThisTurn;
    public (int Self, int Opponent) PokemonTurns;
    public (int Self, int Opponent) ProtectCounter;
    public (int Self, int Opponent) ProtectMovesCount;
    public (bool Self, bool Opponent) Pursuit;
    public (int Self, int Opponent) QuickGuardCounter;
    public (int Self, int Opponent) RageCounter;
    public (int Self, int Opponent) RageFistPower;
    public (int Self, int Opponent) RazorWindCounter;
    public (int Self, int Opponent) Recharge;
    public (int Self, int Opponent) Reflect;
    public (int Self, int Opponent) RolloutCounter;
    public (bool Self, bool Opponent) RoostUsed;
    public (int Self, int Opponent) Safeguard;
    public (int Self, int Opponent) SandTomb;
    public (int Self, int Opponent) ShadowForceCounter;
    public (int Self, int Opponent) SkullBashCounter;
    public (int Self, int Opponent) SkyAttackCounter;
    public (int Self, int Opponent) SkyDropCounter;
    public (int Self, int Opponent) SleepTurns;
    public (int Self, int Opponent) Smacked;
    public (int Self, int Opponent) SolarBeam;
    public (int Self, int Opponent) SolarBlade;
    public (int Self, int Opponent) Spikes;
    public (int Self, int Opponent) SpikyShieldCounter;
    public (int Self, int Opponent) StealthRock;
    public (int Self, int Opponent) StickyWeb;
    public (int Self, int Opponent) StockpileCount;
    public (int Self, int Opponent) Substitute;
    public (int Self, int Opponent) SwapIndex = (-1, -1);
    public (int Self, int Opponent) TailWind;
    public (bool Self, bool Opponent) TarShot;
    public (int Self, int Opponent) Taunt;
    public (int Self, int Opponent) Telekinesis;
    public (int Self, int Opponent) Thrash;
    public (Attack? Self, Attack? Opponent) TormentMove;
    public (int Self, int Opponent) Torment;
    public (int Self, int Opponent) ToxicSpikes;
    public (int Self, int Opponent) TrappedCounter;
    public (int Self, int Opponent) TruantRound;
    public (int Self, int Opponent) TurnCounts;
    public (int Self, int Opponent) Uproar;
    public (bool Self, bool Opponent) UsedBatonPass;
    public (List<int> Self, List<int> Opponent) UsedMoves = ([], []);
    public (bool Self, bool Opponent) UsedMirrorMove;
    public (Attack? Self, Attack? Opponent) UsedMirrorMoveAttack;
    public (bool Self, bool Opponent) UsedRandomMove;
    public (Attack? Self, Attack? Opponent) UsedRandomMoveAttack;
    public (int Self, int Opponent) WaterPledge;
    public (int Self, int Opponent) WideGuardCounter;
    public (int Self, int Opponent) Whirlpool;
    public (int Self, int Opponent) Wish;
    public (int Self, int Opponent) Wrap;
    public (int Self, int Opponent) Yawn;

    // BatonPass data
    public (List<int>? Self, List<int>? Opponent) BatonPassStats;
    public (bool Self, bool Opponent) BatonPassConfusion;
    public (Attack? Self, Attack? Opponent) ChoiceMove;

    // --- Shared counters ---
    private BattleWeather.WeatherTypes _weather = BattleWeather.WeatherTypes.Clear;
    public int WeatherRounds;

    public BattleWeather.WeatherTypes Weather
    {
        get => _weather;
        set
        {
            _weather = value;
            Screen.Level.World.CurrentMapWeather = BattleWeather.GetWorldWeather(value);
        }
    }

    public int TrickRoom;
    public int Gravity;
    public int MudSport;
    public int WaterSport;
    public int Rounds;
    public int AmuletCoin;
    public int ElectricTerrain;
    public int GrassyTerrain;
    public int MistyTerrain;
    public int PsychicTerrain;
    public int TempTripleKick;

    // Special
    public int RunTries;
    public List<int> UsedPokemon = [];
    public Dictionary<int, Item?> StolenFromSelfItems = [];
    public Dictionary<int, Item?> StolenFromOpponentItems = [];
    public bool DefeatedTrainerPokemon;
    public bool RoamingFled;

    // --- Methods ---

    public bool CanUseItem(bool own)
    {
        int embargo = own ? Embargo.Self : Embargo.Opponent;
        if (embargo > 0)
        {
            return false;
        }
        return true;
    }

    public bool CanUseOwnItem(bool own, BattleScreen battleScreen)
    {
        Pokemon p = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        if ("klutz".Equals(p.Ability.Name.ToLower()))
        {
            return false;
        }
        return true;
    }

    public bool CanUseHeldItem(bool own, BattleScreen battleScreen)
    {
        return CanUseItem(own) && CanUseOwnItem(own, battleScreen);
    }

    public bool CanUseAbility(bool own, BattleScreen battleScreen, int checkType = 0)
    {
        if (checkType == 0 || checkType == 2)
        {
            Pokemon p = own ? battleScreen.OpponentPokemon! : battleScreen.SelfPokemon!;
            String[] suppressAbilities = ["mold breaker", "turboblaze", "teravolt"];
            if (suppressAbilities.Contains(p.Ability.Name.ToLower()) == true)
            {
                return false;
            }
        }
        if (checkType == 1 || checkType == 2)
        {
            if (own == true)
            {
                if (GastroAcid.Self == true)
                {
                    return false;
                }
            }
            else
            {
                if (GastroAcid.Opponent == true)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsGrounded(bool own, BattleScreen battleScreen)
    {
        Pokemon p = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        bool grounded = true;

        bool isFlying = p.Type1.Type.Equals(Element.Types.Flying) ||
                        (p.Type2 != null && p.Type2.Type.Equals(Element.Types.Flying));
        bool hasLevitate = "levitate".Equals(p.Ability.Name.ToLower()) &&
                           CanUseAbility(own, battleScreen) == true;

        if (isFlying == true || hasLevitate == true)
        {
            grounded = false;
        }

        int smacked = own ? Smacked.Self : Smacked.Opponent;
        int ingrain = own ? Ingrain.Self : Ingrain.Opponent;
        int telekinesis = own ? Telekinesis.Self : Telekinesis.Opponent;
        int magnetRise = own ? MagnetRise.Self : MagnetRise.Opponent;
        int bounceCounter = own ? BounceCounter.Self : BounceCounter.Opponent;
        int digCounter = own ? DigCounter.Self : DigCounter.Opponent;
        int diveCounter = own ? DiveCounter.Self : DiveCounter.Opponent;
        int flyCounter = own ? FlyCounter.Self : FlyCounter.Opponent;
        int phantomForce = own ? PhantomForceCounter.Self : PhantomForceCounter.Opponent;
        int shadowForce = own ? ShadowForceCounter.Self : ShadowForceCounter.Opponent;
        int skyDrop = own ? SkyDropCounter.Self : SkyDropCounter.Opponent;

        if (Gravity > 0 || smacked > 0 || ingrain > 0)
        {
            grounded = true;
        }
        else
        {
            if (telekinesis > 0 || magnetRise > 0)
            {
                grounded = false;
            }
            if (p.Item != null)
            {
                if ("air balloon".Equals(p.Item.OriginalName.ToLower()) &&
                    CanUseItem(own) == true && CanUseOwnItem(own, battleScreen) == true)
                {
                    grounded = false;
                }
                if ("iron ball".Equals(p.Item.OriginalName.ToLower()) &&
                    CanUseItem(own) == true && CanUseOwnItem(own, battleScreen) == true)
                {
                    grounded = true;
                }
            }
        }

        if (bounceCounter > 0 || digCounter > 0 || diveCounter > 0 || flyCounter > 0 ||
            phantomForce > 0 || shadowForce > 0 || skyDrop > 0)
        {
            grounded = false;
        }

        return grounded;
    }

    public float GetPokemonWeight(bool own, BattleScreen battleScreen)
    {
        Pokemon p = own ? battleScreen.SelfPokemon! : battleScreen.OpponentPokemon!;
        float weight = p.pokedexEntry?.Weight ?? 0f;

        if ("light metal".Equals(p.Ability.Name.ToLower()) && CanUseAbility(own, battleScreen) == true)
        {
            weight /= 2;
        }
        if ("heavy metal".Equals(p.Ability.Name.ToLower()) && CanUseAbility(own, battleScreen) == true)
        {
            weight *= 2;
        }

        return weight;
    }

    public bool MovesFirst(bool own)
    {
        if (own == true)
        {
            return TurnCounts.Self > TurnCounts.Opponent;
        }
        else
        {
            return TurnCounts.Opponent > TurnCounts.Self;
        }
    }
}
