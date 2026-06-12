using System.Collections.Generic;
using P3D.Items;

namespace P3D.BattleSystem;

/// <summary>Represents a Pokemon in battle.</summary>
public class PokemonProfile
{
    private Pokemon _pokemon;
    private PokemonTarget _fieldPosition;

    /// <summary>Creates a new PokemonProfile.</summary>
    public PokemonProfile(PokemonTarget target, Pokemon pokemon)
    {
        _fieldPosition = target;
        _pokemon = pokemon;
    }

    /// <summary>The Pokemon associated with this profile.</summary>
    public Pokemon Pokemon => _pokemon;

    /// <summary>The position of this Pokemon on the field.</summary>
    public PokemonTarget FieldPosition => _fieldPosition;

    // ---- Per-Pokemon battle state (not field-wide effects) ----

    /// <summary>Damage taken in the last hit.</summary>
    public int lastDamageTaken = 0;
    /// <summary>Turns this Pokemon has been in battle.</summary>
    public int turnsInBattle = 0;
    /// <summary>Whether this Pokemon dealt damage to an opponent this turn.</summary>
    public bool dealtDamageThisTurn = false;
    /// <summary>List of move IDs used this battle.</summary>
    public List<int> usedMoves = [];
    /// <summary>Item this Pokemon lost during battle (for Recycle).</summary>
    public Item? lostItem = null;
    /// <summary>Whether this Pokemon has Mega Evolved.</summary>
    public bool megaEvolved = false;
    /// <summary>The last move this Pokemon used.</summary>
    public Attack? lastMove = null;
    /// <summary>Turn number on which this Pokemon last moved.</summary>
    public int lastTurnMoved = 0;
    /// <summary>True if another Pokemon used Pursuit while this one is switching out.</summary>
    public bool pursuit = false;

    // ---- Status / condition counters ----
    public int sleepTurns = 0;
    public int toxicRound = 0;
    public int yawn = 0;
    public int confusionTurns = 0;
    public int truantRound = 0;
    public bool imprisoned = false;
    public int taunted = 0;
    public int telekinesis = 0;
    public int embargo = 0;
    public int encore = 0;
    public Attack? encoreMove = null;
    public int torment = 0;
    public int tormentMoveID = -1;
    public Attack? choiceMove = null;
    public int gastroAcid = 0;

    // ---- Move counters ----
    public int rage = 0;
    public int defenseCurl = 0;
    public int charge = 0;
    public int minimize = 0;
    public int bide = 0;
    public int bideDamage = 0;
    public int waterPledge = 0;
    public int uproar = 0;
    public int outrage = 0;
    public int thrash = 0;
    public int petalDance = 0;
    public int rollout = 0;
    public int iceBall = 0;
    public int recharge = 0;
    public int razorWind = 0;
    public int skullBash = 0;
    public int skyAttack = 0;
    public int solarBeam = 0;
    public int solarBlade = 0;
    public int iceBurn = 0;
    public int freezeShock = 0;
    public int fly = 0;
    public int dig = 0;
    public int bounce = 0;
    public int dive = 0;
    public int shadowForce = 0;
    public int phantomForce = 0;
    public int skyDrop = 0;
    public int geomancy = 0;

    // ---- Trap counters ----
    public int wrap = 0;
    public int whirlpool = 0;
    public int bind = 0;
    public int clamp = 0;
    public int fireSpin = 0;
    public int magmaStorm = 0;
    public int sandTomb = 0;

    // ---- Misc ----
    public int focusEnergy = 0;
    public int luckyChant = 0;
    public int lockedOn = 0;
    public int furyCutter = 0;
    public int echoedVoice = 0;
    public int stockPile = 0;
    public int magicCoat = 0;
    public int roost = 0;
    public int destinyBond = 0;
    public int endure = 0;
    public int protect = 0;
    public int detect = 0;
    public int substitute = 0;
    public int protectMoveCounter = 0;
    public int kingsShield = 0;
    public int spikyShield = 0;
    public int banefulBunker = 0;
    public int craftyShield = 0;
    public int matBlock = 0;
    public int wideGuard = 0;
    public int quickGuard = 0;
    public int ingrain = 0;
    public int magnetRise = 0;
    public int aquaRing = 0;
    public int nightmare = 0;
    public int cursed = 0;
    public int smacked = 0;
    public int perishSong = 0;
    public int trapped = 0;
    public int foresight = 0;
    public int odorSleuth = 0;
    public int miracleEye = 0;
    public int grassPledge = 0;
    public int firePledge = 0;
    public int leechSeed = 0;
    public PokemonTarget? leechSeedTarget = null;
    public int metronomeItemCount = 0;
    public int lansatBerry = 0;
    public int custapBerry = 0;

    /// <summary>
    /// Resets all per-switch-in counters. When <paramref name="batonPassed"/> is true,
    /// stat-accumulation effects (Focus Energy, Ingrain, Substitute, etc.) are preserved.
    /// </summary>
    public void ResetFields(bool batonPassed)
    {
        sleepTurns = 0;
        truantRound = 0;
        taunted = 0;
        telekinesis = 0;
        rage = 0;
        uproar = 0;
        endure = 0;
        protect = 0;
        detect = 0;
        kingsShield = 0;
        spikyShield = 0;
        banefulBunker = 0;
        craftyShield = 0;
        matBlock = 0;
        wideGuard = 0;
        quickGuard = 0;
        protectMoveCounter = 0;
        toxicRound = 0;
        nightmare = 0;
        outrage = 0;
        thrash = 0;
        petalDance = 0;
        encore = 0;
        encoreMove = null;
        yawn = 0;
        confusionTurns = 0;
        torment = 0;
        tormentMoveID = 0;
        choiceMove = null;
        recharge = 0;
        rollout = 0;
        iceBall = 0;
        defenseCurl = 0;
        charge = 0;
        solarBeam = 0;
        solarBlade = 0;
        lansatBerry = 0;
        custapBerry = 0;
        trapped = 0;
        furyCutter = 0;
        echoedVoice = 0;
        turnsInBattle = 0;
        stockPile = 0;
        destinyBond = 0;
        gastroAcid = 0;
        foresight = 0;
        odorSleuth = 0;
        miracleEye = 0;
        fly = 0;
        dig = 0;
        bounce = 0;
        dive = 0;
        shadowForce = 0;
        phantomForce = 0;
        skyDrop = 0;
        geomancy = 0;
        skyAttack = 0;
        razorWind = 0;
        skullBash = 0;
        wrap = 0;
        whirlpool = 0;
        bind = 0;
        clamp = 0;
        fireSpin = 0;
        magmaStorm = 0;
        sandTomb = 0;
        bide = 0;
        bideDamage = 0;
        roost = 0;
        smacked = 0;

        if (batonPassed == false)
        {
            focusEnergy = 0;
            ingrain = 0;
            substitute = 0;
            magnetRise = 0;
            aquaRing = 0;
            cursed = 0;
            embargo = 0;
            perishSong = 0;
            leechSeed = 0;
            leechSeedTarget = null;
        }
    }
}

/// <summary>Represents a position on the battle field by targeting a Pokemon.</summary>
public class PokemonTarget
{
    public enum Targets
    {
        OwnLeft,
        OwnCenter,
        OwnRight,
        OppLeft,
        OppCenter,
        OppRight
    }

    private Targets _target = Targets.OwnCenter;

    public PokemonTarget(Targets t)
    {
        _target = t;
    }

    public PokemonTarget(String s)
    {
        switch (s.ToLower())
        {
            case "ownleft":
                _target = Targets.OwnLeft;
                break;
            case "ownright":
                _target = Targets.OwnRight;
                break;
            case "owncenter":
                _target = Targets.OwnCenter;
                break;
            case "oppleft":
                _target = Targets.OppLeft;
                break;
            case "oppright":
                _target = Targets.OppRight;
                break;
            case "oppcenter":
                _target = Targets.OppCenter;
                break;
        }
    }

    /// <summary>The targeting enum value.</summary>
    public Targets Target => _target;

    public static bool operator ==(PokemonTarget? first, PokemonTarget? second)
    {
        if ((Object?)first == null && (Object?)second == null)
        {
            return true;
        }
        if ((Object?)first == null || (Object?)second == null)
        {
            return false;
        }
        return first._target == second._target;
    }

    public static bool operator !=(PokemonTarget? first, PokemonTarget? second) => (first == second) == false;

    public override bool Equals(Object? obj) => obj is PokemonTarget other && _target == other._target;
    public override int GetHashCode() => _target.GetHashCode();

    // ---- Static factory properties ----
    public static PokemonTarget OwnLeft => new PokemonTarget(Targets.OwnLeft);
    public static PokemonTarget OwnCenter => new PokemonTarget(Targets.OwnCenter);
    public static PokemonTarget OwnRight => new PokemonTarget(Targets.OwnRight);
    public static PokemonTarget OppLeft => new PokemonTarget(Targets.OppLeft);
    public static PokemonTarget OppCenter => new PokemonTarget(Targets.OppCenter);
    public static PokemonTarget OppRight => new PokemonTarget(Targets.OppRight);

    /// <summary>Reverses the sides of the target (Own ↔ Opp).</summary>
    public void Reverse()
    {
        switch (_target)
        {
            case Targets.OwnLeft:
                _target = Targets.OppLeft;
                break;
            case Targets.OwnCenter:
                _target = Targets.OppCenter;
                break;
            case Targets.OwnRight:
                _target = Targets.OppRight;
                break;
            case Targets.OppLeft:
                _target = Targets.OwnLeft;
                break;
            case Targets.OppCenter:
                _target = Targets.OwnCenter;
                break;
            case Targets.OppRight:
                _target = Targets.OwnRight;
                break;
        }
    }

    public override String ToString() => _target.ToString();

    /// <summary>
    /// Converts an attack-targeting enum into the list of reachable PokemonTargets
    /// from this position. Does not filter for occupied slots — call
    /// <see cref="GetValidPokemonTargets"/> afterwards.
    /// </summary>
    public List<PokemonTarget> ConvertAttackTarget(Attack.Targets attackTarget)
    {
        if (attackTarget == Attack.Targets.All)
        {
            return [OwnLeft, OwnCenter, OwnRight, OppLeft, OppCenter, OppRight];
        }
        if (attackTarget == Attack.Targets.Self)
        {
            return [this];
        }

        switch (_target)
        {
            case Targets.OwnLeft:
                switch (attackTarget)
                {
                    case Attack.Targets.OneAdjacentTarget:
                    case Attack.Targets.AllAdjacentTargets:
                        return [OwnCenter, OppLeft, OppCenter];
                    case Attack.Targets.OneAdjacentFoe:
                    case Attack.Targets.AllAdjacentFoes:
                        return [OppLeft, OppCenter];
                    case Attack.Targets.OneAdjacentAlly:
                    case Attack.Targets.AllAdjacentAllies:
                        return [OwnCenter];
                    case Attack.Targets.OneTarget:
                    case Attack.Targets.AllTargets:
                        return [OwnCenter, OwnRight, OppLeft, OppCenter, OppRight];
                    case Attack.Targets.OneFoe:
                    case Attack.Targets.AllFoes:
                        return [OppLeft, OppCenter, OppRight];
                    case Attack.Targets.OneAlly:
                    case Attack.Targets.AllAllies:
                        return [OwnCenter, OwnRight];
                    case Attack.Targets.AllOwn:
                        return [OwnCenter, OwnRight, OwnLeft];
                }
                break;
            case Targets.OwnCenter:
                switch (attackTarget)
                {
                    case Attack.Targets.OneAdjacentTarget:
                    case Attack.Targets.OneTarget:
                    case Attack.Targets.AllAdjacentTargets:
                    case Attack.Targets.AllTargets:
                        return [OwnLeft, OwnRight, OppLeft, OppCenter, OppRight];
                    case Attack.Targets.OneAdjacentFoe:
                    case Attack.Targets.OneFoe:
                    case Attack.Targets.AllAdjacentFoes:
                    case Attack.Targets.AllFoes:
                        return [OppLeft, OppCenter, OppRight];
                    case Attack.Targets.OneAdjacentAlly:
                    case Attack.Targets.OneAlly:
                    case Attack.Targets.AllAdjacentAllies:
                    case Attack.Targets.AllAllies:
                        return [OwnLeft, OwnRight];
                    case Attack.Targets.AllOwn:
                        return [OwnCenter, OwnRight, OwnLeft];
                }
                break;
            case Targets.OwnRight:
                switch (attackTarget)
                {
                    case Attack.Targets.OneAdjacentTarget:
                    case Attack.Targets.AllAdjacentTargets:
                        return [OwnCenter, OppCenter, OppRight];
                    case Attack.Targets.OneAdjacentFoe:
                    case Attack.Targets.AllAdjacentFoes:
                        return [OppCenter, OppRight];
                    case Attack.Targets.OneAdjacentAlly:
                    case Attack.Targets.AllAdjacentAllies:
                        return [OwnCenter];
                    case Attack.Targets.OneTarget:
                    case Attack.Targets.AllTargets:
                        return [OwnLeft, OwnCenter, OppLeft, OppCenter, OppRight];
                    case Attack.Targets.OneFoe:
                    case Attack.Targets.AllFoes:
                        return [OppLeft, OppCenter, OppRight];
                    case Attack.Targets.OneAlly:
                    case Attack.Targets.AllAllies:
                        return [OwnLeft, OwnCenter];
                    case Attack.Targets.AllOwn:
                        return [OwnCenter, OwnRight, OwnLeft];
                }
                break;
            case Targets.OppLeft:
                switch (attackTarget)
                {
                    case Attack.Targets.OneAdjacentTarget:
                    case Attack.Targets.AllAdjacentTargets:
                        return [OwnLeft, OwnCenter, OppCenter];
                    case Attack.Targets.OneAdjacentFoe:
                    case Attack.Targets.AllAdjacentFoes:
                        return [OwnLeft, OwnCenter];
                    case Attack.Targets.OneAdjacentAlly:
                    case Attack.Targets.AllAdjacentAllies:
                        return [OppCenter];
                    case Attack.Targets.OneTarget:
                    case Attack.Targets.AllTargets:
                        return [OwnLeft, OwnCenter, OwnRight, OppCenter, OppRight];
                    case Attack.Targets.OneFoe:
                    case Attack.Targets.AllFoes:
                        return [OwnLeft, OwnCenter, OwnRight];
                    case Attack.Targets.OneAlly:
                    case Attack.Targets.AllAllies:
                        return [OppCenter, OppRight];
                    case Attack.Targets.AllOwn:
                        return [OppCenter, OppRight, OppLeft];
                }
                break;
            case Targets.OppCenter:
                switch (attackTarget)
                {
                    case Attack.Targets.OneAdjacentTarget:
                    case Attack.Targets.OneTarget:
                    case Attack.Targets.AllAdjacentTargets:
                    case Attack.Targets.AllTargets:
                        return [OwnLeft, OwnCenter, OwnRight, OppLeft, OppRight];
                    case Attack.Targets.OneAdjacentFoe:
                    case Attack.Targets.OneFoe:
                    case Attack.Targets.AllAdjacentFoes:
                    case Attack.Targets.AllFoes:
                        return [OwnLeft, OwnCenter, OwnRight];
                    case Attack.Targets.OneAdjacentAlly:
                    case Attack.Targets.OneAlly:
                    case Attack.Targets.AllAdjacentAllies:
                    case Attack.Targets.AllAllies:
                        return [OppLeft, OppRight];
                    case Attack.Targets.AllOwn:
                        return [OppCenter, OppRight, OppLeft];
                }
                break;
            case Targets.OppRight:
                switch (attackTarget)
                {
                    case Attack.Targets.OneAdjacentTarget:
                    case Attack.Targets.AllAdjacentTargets:
                        return [OwnCenter, OwnRight, OppCenter];
                    case Attack.Targets.OneAdjacentFoe:
                    case Attack.Targets.AllAdjacentFoes:
                        return [OwnCenter, OwnRight];
                    case Attack.Targets.OneAdjacentAlly:
                    case Attack.Targets.AllAdjacentAllies:
                        return [OppCenter];
                    case Attack.Targets.OneTarget:
                    case Attack.Targets.AllTargets:
                        return [OwnLeft, OwnCenter, OwnRight, OppRight, OppCenter];
                    case Attack.Targets.OneFoe:
                    case Attack.Targets.AllFoes:
                        return [OwnLeft, OwnCenter, OwnRight];
                    case Attack.Targets.OneAlly:
                    case Attack.Targets.AllAllies:
                        return [OppLeft, OppCenter];
                    case Attack.Targets.AllOwn:
                        return [OppCenter, OppRight, OppLeft];
                }
                break;
        }

        return [];
    }

    /// <summary>
    /// Filters a list of PokemonTargets to only those that have an assigned profile
    /// on the battle field.
    /// </summary>
    public static List<PokemonTarget> GetValidPokemonTargets(BattleScreen battleScreen,
                                                              List<PokemonTarget> l)
    {
        List<PokemonTarget> returnList = [];
        foreach (PokemonTarget target in l)
        {
            if (battleScreen.GetProfile(target) != null)
            {
                returnList.Add(target);
            }
        }
        return returnList;
    }

    /// <summary>True if this target is on the player's (own) side of the field.</summary>
    public bool IsOwn
    {
        get
        {
            switch (_target)
            {
                case Targets.OwnCenter:
                case Targets.OwnLeft:
                case Targets.OwnRight:
                    return true;
                case Targets.OppCenter:
                case Targets.OppLeft:
                case Targets.OppRight:
                    return false;
            }
            return true;
        }
    }
}
