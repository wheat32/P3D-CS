using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;
using P3D.Items;

namespace P3D.BattleSystem;

// ---------------------------------------------------------------------------
// TODO Phase 5: replace all stubs below with fully ported implementations
// ---------------------------------------------------------------------------

// ---- Battle step stubs (used by moves like Encore that check what the opponent just did) ----
public class BattleStepTypes
{
    // Static access: BattleStepTypes.Move
    public static readonly String Move = "Move";
    public static readonly String Switch = "Switch";
    public static readonly String Other = "Other";

    // Instance access aliases (VB used instance.StepTypes.Move pattern)
    public String MoveStep => Move;
    public String SwitchStep => Switch;
    public String OtherStep => Other;
}

public class BattleRoundConst
{
    public BattleStepTypes StepTypes { get; } = new BattleStepTypes();
}

public class BattleStep
{
    public String StepType = BattleStepTypes.Move;
    public Object? Argument;
}

// ---- Weather enum (used by FieldEffects.Weather and ChangeWeather calls) ----
public static class BattleWeather
{
    public enum WeatherTypes
    {
        Clear,
        Rain,
        Sandstorm,
        Hailstorm,
        Sunny,
        Shadow,
        Fog,
        Undefined
    }
}

// ---- BattleMenu stub ----
public class BattleMenu
{
    public bool Visible { get; set; }
}

// ---- BattleScreen ----
public class BattleScreen : Screen
{
    public Pokemon? SelfPokemon { get; set; }
    public Pokemon? OpponentPokemon { get; set; }
    public NPC? SelfPokemonNPC { get; set; }
    public NPC? OpponentPokemonNPC { get; set; }
    public FieldEffects FieldEffects { get; } = new FieldEffects();
    public Battle Battle { get; } = new Battle();
    public bool IsPVPBattle;
    public bool IsTrainerBattle;
    public bool IsRemoteBattle;
    public int SelfPokemonIndex;
    public int OpponentPokemonIndex;
    public NPC? Trainer;
    public List<QueryObject> BattleQuery { get; } = [];
    public BattleMenu BattleMenu { get; } = new BattleMenu();

    // Battle rule flags (set per-map via SetBattleVariables)
    public static bool CanRun = true;
    public static bool CanAlwaysRun;
    public static bool CanCatch = true;
    public static bool CanBlackout = true;
    public static bool CanReceiveEXP = true;
    public static bool CanUseItems = true;
    public static bool DiveBattle;
    public static bool IsInverseBattle;
    public static String CustomBattleMusic = "";
    public static bool CanGainLoseMoney = true;
    public static bool RoamingBattle;
    public static Pokemon? RoamingPokemonStorage;

    public BattleScreen(Pokemon wildPokemon, Screen preScreen, Spawner.EncounterMethods method)
    {
        PreScreen = preScreen;
        Identification = Identifications.BattleScreen;
        OpponentPokemon = wildPokemon;
    }

    public BattleScreen(Trainer trainer, Screen preScreen, int introType)
    {
        PreScreen = preScreen;
        Identification = Identifications.BattleScreen;
    }
}

// ---- FieldEffects ----
public class FieldEffects
{
    // --- Shared counters ---
    public BattleWeather.WeatherTypes Weather;
    public int ElectricTerrain;
    public int GrassyTerrain;
    public int MistyTerrain;
    public int PsychicTerrain;
    public int Gravity;
    public int MudSport;
    public int WaterSport;
    public int TrickRoom;
    public int TempTripleKick;
    public bool MovesFirst(bool own) => false;
    public Dictionary<int, Item?> StolenFromSelfItems = [];
    public Dictionary<int, Item?> StolenFromOpponentItems = [];

    // --- Per-side counters: (Self, Opponent) ---
    public (int Self, int Opponent) AquaRing;
    public (int Self, int Opponent) BanefulBunkerCounter;
    public (int Self, int Opponent) BatonPassIndex;
    public (int Self, int Opponent) BideCounter;
    public (int Self, int Opponent) BideDamage;
    public (int Self, int Opponent) Bind;
    public (int Self, int Opponent) BounceCounter;
    public (int Self, int Opponent) Charge;
    public (int Self, int Opponent) Clamp;
    public (Item? Self, Item? Opponent) ConsumedItem;
    public (int Self, int Opponent) CraftyShieldCounter;
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
    public (int Self, int Opponent) FlyCounter;
    public (int Self, int Opponent) FocusEnergy;
    public (int Self, int Opponent) Foresight;
    public (int Self, int Opponent) FreezeShockCounter;
    public (int Self, int Opponent) FuryCutter;
    public (int Self, int Opponent) FutureSightDamage;
    public (int Self, int Opponent) FutureSightID;
    public (int Self, int Opponent) FutureSightTurns;
    public (int Self, int Opponent) GeomancyCounter;
    public (int Self, int Opponent) GrassPledge;
    public (int Self, int Opponent) HealBlock;
    public (bool Self, bool Opponent) HealingWish;
    public (int Self, int Opponent) IceBallCounter;
    public (int Self, int Opponent) IceBurnCounter;
    public (int Self, int Opponent) Infestation;
    public (int Self, int Opponent) Ingrain;
    public (int Self, int Opponent) KingsShieldCounter;
    public (int Self, int Opponent) LastDamage;
    public (Attack? Self, Attack? Opponent) LastMove;
    public (bool Self, bool Opponent) LastMoveFailed;
    public (int Self, int Opponent) LeechSeed;
    public (int Self, int Opponent) LightScreen;
    public (int Self, int Opponent) LockOn;
    public (int Self, int Opponent) MagicCoat;
    public (int Self, int Opponent) MagmaStorm;
    public (int Self, int Opponent) MagnetRise;
    public (int Self, int Opponent) MatBlockCounter;
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
    public (int Self, int Opponent) Smacked;
    public (int Self, int Opponent) SolarBeam;
    public (int Self, int Opponent) SolarBlade;
    public (int Self, int Opponent) Spikes;
    public (int Self, int Opponent) SpikyShieldCounter;
    public (int Self, int Opponent) StealthRock;
    public (bool Self, bool Opponent) TarShot;
    public (int Self, int Opponent) StickyWeb;
    public (int Self, int Opponent) StockpileCount;
    public (int Self, int Opponent) Substitute;
    public (int Self, int Opponent) SwapIndex;
    public (int Self, int Opponent) TailWind;
    public (int Self, int Opponent) Taunt;
    public (int Self, int Opponent) Thrash;
    public (int Self, int Opponent) Torment;
    public (int Self, int Opponent) ToxicSpikes;
    public (int Self, int Opponent) TrappedCounter;
    public (int Self, int Opponent) TurnCounts;
    public (int Self, int Opponent) Uproar;
    public (bool Self, bool Opponent) UsedBatonPass;
    public (List<int> Self, List<int> Opponent) UsedMoves = ([], []);
    public (int Self, int Opponent) WaterPledge;
    public (int Self, int Opponent) Whirlpool;
    public (int Self, int Opponent) WideGuardCounter;
    public (int Self, int Opponent) Wish;
    public (int Self, int Opponent) Wrap;
    public (int Self, int Opponent) Yawn;

    // --- Methods ---
    public bool CanUseAbility(bool own, BattleScreen battleScreen) => true;
    public bool CanUseItem(bool own) => true;
    public bool CanUseHeldItem(bool own, BattleScreen battleScreen) => true;
    public float GetPokemonWeight(bool own, BattleScreen battleScreen) => 0f;
}

// ---- Battle (all combat logic stubs) ----
public class Battle
{
    // Battle-round step objects accessed statically in VB (TODO Phase 5: full port)
    public static BattleStep SelfStep { get; } = new BattleStep();
    public static BattleStep OpponentStep { get; } = new BattleStep();
    public static BattleRoundConst RoundConst { get; } = new BattleRoundConst();
    public bool RaiseStat(bool target, bool own, BattleScreen battleScreen,
                          String stat, int stages, String message, String caller) => true;
    public bool LowerStat(bool target, bool own, BattleScreen battleScreen,
                          String stat, int stages, String message, String caller) => true;
    public void ReduceHP(int hp, bool target, bool own, BattleScreen battleScreen,
                         String message, String caller) { }
    public void GainHP(int hp, bool target, bool own, BattleScreen battleScreen,
                       String message, String caller) { }
    public void FaintPokemon(bool target, bool own, BattleScreen battleScreen,
                              String message, String caller) { }
    public bool InflictBurn(bool target, bool own, BattleScreen battleScreen,
                            String message, String caller) => true;
    public bool InflictSleep(bool target, bool own, BattleScreen battleScreen,
                              int turns, String message, String caller) => true;
    public bool InflictParalysis(bool target, bool own, BattleScreen battleScreen,
                                  String message, String caller) => true;
    public bool InflictPoison(bool target, bool own, BattleScreen battleScreen,
                               bool bad, String message, String caller) => true;
    public bool InflictFreeze(bool target, bool own, BattleScreen battleScreen,
                               String message, String caller) => true;
    public bool InflictConfusion(bool target, bool own, BattleScreen battleScreen,
                                  String message, String caller) => true;
    public void InflictFlinch(bool target, bool own, BattleScreen battleScreen,
                               String message, String caller) { }
    public void InflictRecoil(bool own, bool target, BattleScreen battleScreen,
                               Attack attack, int damage, String message, String caller) { }
    public bool InflictInfatuate(bool target, bool own, BattleScreen battleScreen,
                                  String message, String caller) => true;
    public void CureStatusProblem(bool target, bool own, BattleScreen battleScreen,
                                   String message, String caller) { }
    public bool RemoveHeldItem(bool target, bool own, BattleScreen battleScreen,
                                String message, String caller) => true;
    public bool RemoveHeldItem(bool target, bool own, BattleScreen battleScreen,
                                String message, String caller, bool steal) => true;
    public void SwitchOutSelf(BattleScreen battleScreen, int index, int swapIndex) { }
    public void SwitchOutOpponent(BattleScreen battleScreen, int index,
                              String message = "", bool show = true) { }
    public void ChangeWeather(bool target, bool own, BattleWeather.WeatherTypes weather,
                               int turns, BattleScreen battleScreen,
                               String message, String caller) { }
    public void ChangeCameraAngle(int angle, bool smooth, BattleScreen battleScreen) { }
    public void DoAttackRound(BattleScreen battleScreen, bool own, Attack move) { }
    public void UseBerry(bool target, bool own, Item? item, BattleScreen battleScreen,
                          String message, String caller) { }
    public bool WildHasEscaped { get; set; }
    public void SetWildHasEscaped(BattleScreen battleScreen) { }
}

// ---- BattleCalculation (extended) ----
public static class BattleCalculation
{
    public static int CalculateDamage(Attack attack, bool critical, bool own,
                                       bool targetPokemon, BattleScreen battleScreen,
                                       String extraParam = "",
                                       Attack? typeEffectivenessAttack = null) => attack.Power;
    public static bool CanSwitch(BattleScreen battleScreen, bool own) => true;
    public static bool CanRun(BattleScreen battleScreen, bool own) => true;
    public static bool CanRun(bool own, BattleScreen battleScreen, bool checkTrapped) => true;
    public static int DetermineBattleAttack(bool own, BattleScreen battleScreen) => 0;
    public static int DetermineBattleSpeed(bool own, BattleScreen battleScreen) => 0;
    public static int FieldEffectTurns(BattleScreen battleScreen, bool own,
                                        String effectName) => 5;
    public static float ReverseTypeEffectiveness(float multiplier) => 1f / multiplier;
}

// ---- AttackSpecialBasePower ----
public static class AttackSpecialBasePower
{
    public static int GetGameModeBasePower(Attack attack, bool own,
                                            BattleScreen battleScreen) => attack.Power;
}

// ---- AttackSpecialFunctions ----
public static class AttackSpecialFunctions
{
    public static void ExecuteMoveHitsFunction(Attack attack, bool own,
                                                BattleScreen battleScreen) { }
}

// ---- QueryObjects ----

public abstract class QueryObject
{
    public virtual bool IsReady => true;
}

public class TextQueryObject : QueryObject
{
    public TextQueryObject(String text) { }
}

public class EndBattleQueryObject : QueryObject
{
    public EndBattleQueryObject(bool playerEscaped) { }
}

public class ToggleEntityQueryObject : QueryObject
{
    public enum BattleEntities
    {
        SelfPokemon,
        OpponentPokemon,
        SelfTrainer,
        OpponentTrainer,
    }

    public ToggleEntityQueryObject(bool own, BattleEntities entity, String spriteName,
                                    int frameRow, int frameCount, int startX, int startY) { }
}

// ---- AnimationQueryObject ----
public class AnimationQueryObject : QueryObject
{
    public bool DrawBeforeEntities;

    public AnimationQueryObject(Entity entity, bool battleFlip) { }
    public AnimationQueryObject(NPC entity, bool battleFlip) { }
    public AnimationQueryObject(Entity entity, bool battleFlip, String extraParam) { }
    public AnimationQueryObject(NPC entity, bool battleFlip, bool drawBeforeEntities)
    {
        DrawBeforeEntities = drawBeforeEntities;
    }

    public void AnimationPlaySound(String sound, double minPitch, double maxPitch) { }
    public void AnimationPlaySound(String sound, double minPitch, double maxPitch, bool loop) { }

    // SpawnEntity returns Entity (params Object[] accepts any arg combination)
    public Entity SpawnEntity(params Object[] args) => null!;

    // All remaining animation methods use params Object[] (Phase 5 stub)
    public void AnimationFade(params Object[] args) { }
    public void AnimationMove(params Object[] args) { }
    public void AnimationOscillateMove(params Object[] args) { }
    public void AnimationCameraOscillateMove(params Object[] args) { }
    public void AnimationRotate(params Object[] args) { }
    public void AnimationScale(params Object[] args) { }
    public void AnimationColor(params Object[] args) { }
    public void AnimationChangeTexture(params Object[] args) { }
    public void AnimationBackground(params Object[] args) { }
    public void AnimationTurnNPC(params Object[] args) { }
}

// TODO Phase 5: full Trainer port
public class Trainer
{
    public static int FrontierTrainer;

    public String IntroMessage { get; set; } = "";
    public String DefeatMessage { get; set; } = "";
    public int IntroType { get; set; }

    public Trainer(String id) { }

    public bool IsBeaten() => false;
    public String GetInSightMusic() => "";
    public String GetIniMusicName() => "";
}
