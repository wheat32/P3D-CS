using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem;

public class Attack
{
    // Constants

    public const int MOVE_COUNT = 560;

    private const int PP_UP_DIVISOR = 5;
    private const int PP_UP_MAX_STAGES = 3;
    private static readonly (int Min, int Max) PP_BASE_RANGE = (5, 40);

    private const int CHANCE_MAX = 100;

    private const int CATEGORY_SPRITE_X = 115;
    private const int CATEGORY_SPRITE_WIDTH = 28;
    private const int CATEGORY_SPRITE_HEIGHT = 14;
    private const int CATEGORY_PHYSICAL_Y = 0;
    private const int CATEGORY_SPECIAL_Y = 14;
    private const int CATEGORY_STATUS_Y = 28;

    // Enums

    public enum Categories
    {
        Physical,
        Special,
        Status
    }

    public enum ContestCategories
    {
        Tough,
        Smart,
        Beauty,
        Cool,
        Cute
    }

    public enum Targets
    {
        OneAdjacentTarget,
        OneAdjacentFoe,
        OneAdjacentAlly,
        OneTarget,
        OneFoe,
        OneAlly,
        Self,
        AllAdjacentTargets,
        AllAdjacentFoes,
        AllAdjacentAllies,
        AllTargets,
        AllFoes,
        AllAllies,
        All,
        AllOwn
    }

    public enum AIField
    {
        Nothing,
        Damage,
        Poison,
        Burn,
        Paralysis,
        Sleep,
        Freeze,
        Confusion,
        ConfuseOwn,
        CanPoison,
        CanBurn,
        CanParalyze,
        CanSleep,
        CanFreeze,
        CanConfuse,
        RaiseAttack,
        RaiseDefense,
        RaiseSpAttack,
        RaiseSpDefense,
        RaiseSpeed,
        RaiseAccuracy,
        RaiseEvasion,
        LowerAttack,
        LowerDefense,
        LowerSpAttack,
        LowerSpDefense,
        LowerSpeed,
        LowerAccuracy,
        LowerEvasion,
        CanRaiseAttack,
        CanRaiseDefense,
        CanRaiseSpAttack,
        CanRaiseSpDefense,
        CanRaiseSpeed,
        CanRaiseAccuracy,
        CanRauseEvasion,
        CanLowerAttack,
        CanLowerDefense,
        CanLowerSpAttack,
        CanLowerSpDefense,
        CanLowerSpeed,
        CanLowerAccuracy,
        CanLowerEvasion,
        Flinch,
        CanFlinch,
        Infatuation,
        Trap,
        OHKO,
        MultiTurn,
        Recoil,
        Healing,
        CureStatus,
        Support,
        Recharge,
        HighPriority,
        Absorbing,
        Selfdestruct,
        ThrawOut,
        CannotMiss,
        RemoveReflectLightscreen
    }

    // Properties (had explicit getter/setter in VB)

    public int ID { get; set; } = 1;
    public int Power { get; set; } = 40;
    public int Accuracy { get; set; } = 100;
    public String Name { get; set; } = "Pound";

    public String Description
    {
        get
        {
            if (Localization.TokenExists($"move_desc_{ID}") == true)
            {
                return Localization.GetString($"move_desc_{ID}");
            }
            return field;
        }
        set;
    } = "Pounds with forelegs or tail.";

    // Core definition fields

    public Element type = new Element(Element.Types.Normal);

    // PascalCase aliases for VB compatibility in battle method bodies
    public Element Type { get => type; set => type = value; }
    public Categories Category { get => category; set => category = value; }
    // Disabled: VB accessed as PascalCase, C# is camelCase
    public int Disabled { get => disabled; set => disabled = value; }
    public int CurrentPP { get => currentPP; set => currentPP = value; }
    public int MaxPP { get => maxPP; set => maxPP = value; }
    public int OriginalPP { get => originalPP; set => originalPP = value; }
    public int originalID = 1;
    public bool isDefaultMove = false;

    // GameMode fields

    public String gameModeFunction = String.Empty;
    public String gameModeBasePower = String.Empty;
    public bool isGameModeMove = false;
    public bool gmDeductPP = true;
    public int gmCopyMove = -1;
    public String gmTimesToAttack = "1";
    public Attack? gmUseMoveAnims = null;
    public bool gmUseRandomMove = false;
    public List<int> gmRandomMoveList = [];

    // PP and category fields

    public int originalPP = 35;
    public Categories category = Categories.Physical;
    public ContestCategories contestCategory = ContestCategories.Tough;
    public int criticalChance = 1;
    public bool isHMMove = false;
    public Targets target = Targets.OneAdjacentTarget;
    public int priority = 0;
    public int timesToAttack = 1;
    public List<int> effectChances = [];

    // Effect flags

    public bool makesContact = true;
    public bool hasSecondaryEffect = false;
    public bool isHealingMove = false;
    public bool isDamagingMove = true;
    public bool isProtectMove = false;
    public bool isOneHitKOMove = false;
    public bool isRecoilMove = false;
    public bool isTrappingMove = false;
    public bool removesSelfFrozen = false;
    public bool removesOpponentFrozen = false;
    public bool swapsOutSelfPokemon = false;
    public bool swapsOutOpponentPokemon = false;

    // Interaction flags

    public bool protectAffected = true;
    public bool magicCoatAffected = false;
    public bool snatchAffected = false;
    public bool mirrorMoveAffected = true;
    public bool kingsrockAffected = true;
    public bool counterAffected = true;
    public bool isAffectedBySubstitute = true;
    public bool immunityAffected = true;
    public bool isWonderGuardAffected = true;
    public bool disabledWhileGravity = false;
    public bool useEffectiveness = true;

    // Accuracy and positioning flags

    public bool useAccEvasion = true;
    public bool canHitInMidAir = false;
    public bool canHitUnderground = false;
    public bool canHitUnderwater = false;
    public bool canHitSleeping = true;
    public bool canGainSTAB = true;
    public bool useOpponentDefense = true;
    public bool useOpponentEvasion = true;

    // Category flags

    public bool isPulseMove = false;
    public bool isBulletMove = false;
    public bool isJawMove = false;
    public bool isDanceMove = false;
    public bool isExplosiveMove = false;
    public bool isPowderMove = false;
    public bool isPunchingMove = false;
    public bool isSlicingMove = false;
    public bool isSoundMove = false;
    public bool isWindMove = false;

    public bool focusOpponentPokemon = true;

    // Instance state

    public int currentPP = 0;
    public int maxPP = 0;
    public int disabled = 0;

    // AI hints

    public AIField aiField1 = AIField.Damage;
    public AIField aiField2 = AIField.Nothing;
    public AIField aiField3 = AIField.Nothing;

    // -------------------------------------------------------------------------
    // Factory
    // -------------------------------------------------------------------------

    // TODO Phase 5: replace with per-move subclass instantiation (Moves.Normal.Pound() etc.)
    public static Attack GetAttackByID(int id)
    {
        Attack returnMove = new Attack();
        returnMove.isDefaultMove = true;
        returnMove.originalID = id;
        return returnMove;
    }

    public Attack GetRandomAttack()
    {
        int moveID = gmRandomMoveList[Core.Random.Next(0, gmRandomMoveList.Count - 1)];
        return GetAttackByID(moveID);
    }

    // -------------------------------------------------------------------------
    // Battle hooks — virtual, overridden per move
    // -------------------------------------------------------------------------

    public int GetEffectChance(int i, bool own, BattleScreen battleScreen)
    {
        Attack attack = this;
        if (gmCopyMove != -1)
        {
            attack = GetAttackByID(gmCopyMove);
        }

        int chance = attack.effectChances[i];

        if (attack.hasSecondaryEffect == true)
        {
            Pokemon? p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
            if (p != null && p.Ability != null && p.Ability.Name.ToLower().Equals("serene grace"))
            {
                chance *= 2;
            }

            int waterPledge = own ? battleScreen.FieldEffects.WaterPledge.Self : battleScreen.FieldEffects.WaterPledge.Opponent;
            if (waterPledge > 0)
            {
                chance *= 2;
            }
        }

        return chance.Clamp(0, CHANCE_MAX);
    }

    public virtual void PreAttack(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).PreAttack(own, battleScreen);
        }
    }

    public virtual bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            return GetAttackByID(gmCopyMove).MoveFailBeforeAttack(own, battleScreen);
        }
        return false;
    }

    public virtual int GetBasePower(bool own, BattleScreen battleScreen)
    {
        if (isGameModeMove == false)
        {
            return Power;
        }
        if (gmCopyMove != -1)
        {
            Attack copied = GetAttackByID(gmCopyMove);
            if (copied.isGameModeMove == false)
            {
                return copied.GetBasePower(own, battleScreen);
            }
            else
            {
                return AttackSpecialBasePower.GetGameModeBasePower(copied, own, battleScreen);
            }
        }
        return AttackSpecialBasePower.GetGameModeBasePower(this, own, battleScreen);
    }

    public virtual int GetDamage(bool critical, bool own, bool targetPokemon,
                                  BattleScreen battleScreen, String extraParam = "",
                                  Attack? typeEffectivenessAttack = null)
    {
        if (gmCopyMove != -1)
        {
            Attack copied = GetAttackByID(gmCopyMove);
            if (copied.isGameModeMove == false)
            {
                return copied.GetDamage(critical, own, targetPokemon, battleScreen, extraParam, this);
            }
            else
            {
                return BattleCalculation.CalculateDamage(copied, critical, own, targetPokemon, battleScreen, extraParam, this);
            }
        }
        return BattleCalculation.CalculateDamage(this, critical, own, targetPokemon, battleScreen, extraParam, typeEffectivenessAttack);
    }

    public virtual int GetTimesToAttack(bool own, BattleScreen battleScreen)
    {
        if (isGameModeMove == false)
        {
            return timesToAttack;
        }
        Attack resolvedAttack = this;
        if (gmCopyMove != -1)
        {
            Attack copied = GetAttackByID(gmCopyMove);
            if (copied.isGameModeMove == false)
            {
                return copied.GetTimesToAttack(own, battleScreen);
            }
            resolvedAttack = copied;
        }
        if (resolvedAttack.gmTimesToAttack.Contains("-"))
        {
            return Core.Random.Next(
                int.Parse(resolvedAttack.gmTimesToAttack.GetSplit(0, "-")),
                int.Parse(resolvedAttack.gmTimesToAttack.GetSplit(1, "-")) + 1);
        }
        return int.Parse(resolvedAttack.gmTimesToAttack);
    }

    public virtual void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (isGameModeMove == false) return;
        if (gmCopyMove != -1)
        {
            Attack copied = GetAttackByID(gmCopyMove);
            if (copied.isGameModeMove == false)
            {
                copied.MoveHits(own, battleScreen);
            }
            else
            {
                AttackSpecialFunctions.ExecuteMoveHitsFunction(copied, own, battleScreen);
            }
        }
        else
        {
            AttackSpecialFunctions.ExecuteMoveHitsFunction(this, own, battleScreen);
        }
    }

    public virtual void MoveRecoil(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveRecoil(own, battleScreen);
        }
    }

    public virtual void MoveRecharge(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveRecharge(own, battleScreen);
        }
    }

    public virtual void MoveMultiTurn(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveMultiTurn(own, battleScreen);
        }
    }

    public virtual void MoveSwitch(bool own, BattleScreen battleScreen) { }

    public virtual void MoveMisses(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveMisses(own, battleScreen);
        }
    }

    public virtual void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveProtectedDetected(own, battleScreen);
        }
    }

    public virtual void MoveHasNoEffect(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveHasNoEffect(own, battleScreen);
        }
    }

    public virtual Element GetAttackType(bool own, BattleScreen battleScreen)
    {
        Pokemon? p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;

        if (p != null && p.Ability != null)
        {
            String abilityName = p.Ability.Name.ToLower();
            if (abilityName.Equals("normalize"))
            {
                return new Element(Element.Types.Normal);
            }

            if (type.Type == Element.Types.Normal)
            {
                if (abilityName.Equals("pixilate"))
                {
                    return new Element(Element.Types.Fairy);
                }
                else if (abilityName.Equals("refrigerate"))
                {
                    return new Element(Element.Types.Ice);
                }
                else if (abilityName.Equals("aerilate"))
                {
                    return new Element(Element.Types.Flying);
                }
                else if (abilityName.Equals("galvanize"))
                {
                    return new Element(Element.Types.Electric);
                }
            }
        }

        return type;
    }

    public virtual int GetAccuracy(bool own, BattleScreen battleScreen)
    {
        return Accuracy;
    }

    public virtual bool DeductPP(bool own, BattleScreen battleScreen)
    {
        if (isGameModeMove == false) return true;
        if (gmCopyMove != -1)
        {
            Attack copied = GetAttackByID(gmCopyMove);
            if (copied.isGameModeMove == false)
            {
                return copied.DeductPP(own, battleScreen);
            }
            else
            {
                return copied.gmDeductPP;
            }
        }
        return gmDeductPP;
    }

    public virtual bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        return useAccEvasion;
    }

    public virtual void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveSelected(own, battleScreen);
        }
    }

    public virtual void BeforeDealingDamage(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).BeforeDealingDamage(own, battleScreen);
        }
    }

    public virtual void AbsorbedBySubstitute(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).AbsorbedBySubstitute(own, battleScreen);
        }
    }

    public virtual void MoveFailsSoundproof(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).MoveFailsSoundproof(own, battleScreen);
        }
    }

    public virtual void InflictedFlinch(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).InflictedFlinch(own, battleScreen);
        }
    }

    public virtual void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).HurtItselfInConfusion(own, battleScreen);
        }
    }

    public virtual void IsAttracted(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).IsAttracted(own, battleScreen);
        }
    }

    public virtual void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).IsParalyzed(own, battleScreen);
        }
    }

    public virtual void IsSleeping(bool own, BattleScreen battleScreen)
    {
        if (gmCopyMove != -1)
        {
            GetAttackByID(gmCopyMove).IsSleeping(own, battleScreen);
        }
    }

    public virtual int GetUseAttackStat(Pokemon p)
    {
        if (category == Categories.Physical)
        {
            return p.Attack;
        }
        return p.SpAttack;
    }

    public virtual int GetUseDefenseStat(Pokemon p)
    {
        if (category == Categories.Physical)
        {
            return p.Defense;
        }
        return p.SpDefense;
    }

    public bool AIUseMove(BattleScreen battleScreen)
    {
        return true;
    }

    // -------------------------------------------------------------------------
    // Move animations — virtual, overridden per move
    // -------------------------------------------------------------------------

    public void UserPokemonMoveAnimation(BattleScreen battleScreen, bool own)
    {
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            bool battleFlip = false;
            Pokemon? currentPokemon = battleScreen.SelfPokemon;
            NPC? currentEntity = battleScreen.SelfPokemonNPC;
            if (own == false)
            {
                battleFlip = true;
                currentPokemon = battleScreen.OpponentPokemon;
                currentEntity = battleScreen.OpponentPokemonNPC;
            }
            if (currentPokemon != null && currentEntity != null)
            {
                InternalUserPokemonMoveAnimation(battleScreen, battleFlip, currentPokemon, currentEntity);
            }
        }
    }

    public virtual void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip,
                                                          Pokemon currentPokemon, NPC currentEntity)
    {
        if (isGameModeMove == true && gmUseMoveAnims != null)
        {
            gmUseMoveAnims.InternalUserPokemonMoveAnimation(battleScreen, battleFlip, currentPokemon, currentEntity);
        }
    }

    public void OpponentPokemonMoveAnimation(BattleScreen battleScreen, bool own)
    {
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            bool battleFlip = false;
            Pokemon? currentPokemon = battleScreen.OpponentPokemon;
            NPC? currentEntity = battleScreen.OpponentPokemonNPC;
            if (own == false)
            {
                battleFlip = true;
                currentPokemon = battleScreen.SelfPokemon;
                currentEntity = battleScreen.SelfPokemonNPC;
            }
            if (currentPokemon != null && currentEntity != null)
            {
                InternalOpponentPokemonMoveAnimation(battleScreen, battleFlip, currentPokemon, currentEntity);
            }
        }
    }

    public virtual void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip,
                                                              Pokemon currentPokemon, NPC currentEntity)
    {
        if (isGameModeMove == true && gmUseMoveAnims != null)
        {
            gmUseMoveAnims.InternalOpponentPokemonMoveAnimation(battleScreen, battleFlip, currentPokemon, currentEntity);
        }
    }

    public void FailPokemonMoveAnimation(BattleScreen battleScreen, bool own)
    {
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            bool battleFlip = false;
            Pokemon? currentPokemon = battleScreen.SelfPokemon;
            NPC? currentEntity = battleScreen.SelfPokemonNPC;
            if (own == false)
            {
                battleFlip = true;
                currentPokemon = battleScreen.OpponentPokemon;
                currentEntity = battleScreen.OpponentPokemonNPC;
            }
            if (currentPokemon != null && currentEntity != null)
            {
                InternalFailPokemonMoveAnimation(battleScreen, battleFlip, currentPokemon, currentEntity);
            }
        }
    }

    public virtual void InternalFailPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip,
                                                          Pokemon currentPokemon, NPC currentEntity)
    {
        if (isGameModeMove == true && gmUseMoveAnims != null)
        {
            gmUseMoveAnims.InternalFailPokemonMoveAnimation(battleScreen, battleFlip, currentPokemon, currentEntity);
        }
    }

    // -------------------------------------------------------------------------
    // Utility
    // -------------------------------------------------------------------------

    public Attack Copy()
    {
        Attack m;
        if (isGameModeMove == true)
        {
            m = GameModeAttackLoader.GetAttackByID(ID);
        }
        else
        {
            m = GetAttackByID(ID);
        }

        m.originalPP = originalPP;
        m.currentPP = currentPP;
        m.maxPP = maxPP;
        m.originalID = originalID;

        return m;
    }

    public static Attack? ConvertStringToAttack(String inputData)
    {
        if (String.IsNullOrEmpty(inputData)) return null;

        String[] data = inputData.Split(',');
        Attack? a = GetAttackByID(int.Parse(data[0]));
        if (a != null && data.Length == 3)
        {
            a.maxPP = int.Parse(data[1]);
            a.currentPP = int.Parse(data[2]);
        }
        return a;
    }

    public bool RaisePP()
    {
        if (originalPP % PP_UP_DIVISOR != 0 || originalPP < PP_BASE_RANGE.Min || originalPP > PP_BASE_RANGE.Max)
        {
            currentPP = Math.Clamp(currentPP, 0, maxPP);
            return false;
        }

        int increment = originalPP / PP_UP_DIVISOR;
        int maxAllowed = originalPP + PP_UP_MAX_STAGES * increment;
        if (maxPP < maxAllowed)
        {
            currentPP += increment;
            maxPP += increment;
            return true;
        }

        currentPP = Math.Clamp(currentPP, 0, maxPP);
        return false;
    }

    public Texture2D GetDamageCategoryImage()
    {
        int spriteY;
        switch (category)
        {
            case Categories.Physical:
                spriteY = CATEGORY_PHYSICAL_Y;
                break;
            case Categories.Special:
                spriteY = CATEGORY_SPECIAL_Y;
                break;
            default:
                spriteY = CATEGORY_STATUS_Y;
                break;
        }

        Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(
            CATEGORY_SPRITE_X, spriteY, CATEGORY_SPRITE_WIDTH, CATEGORY_SPRITE_HEIGHT);
        return TextureManager.GetTexture(Element.GetElementTexturePath(), r, "");
    }

    public override String ToString()
    {
        return $"{originalID},{maxPP},{currentPP}";
    }
}
