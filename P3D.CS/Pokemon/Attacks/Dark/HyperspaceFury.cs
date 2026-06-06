using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class HyperspaceFury : Attack
{
    public HyperspaceFury()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 621;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 100;
        Accuracy = 0;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Hyperspace Fury");
        Description = "Using its many arms, the user unleashes a barrage of attacks that ignore the effects of moves like Protect and Detect. But the user's Defense stat falls.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = false;
        kingsrockAffected = true;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = true;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        useAccEvasion = false;
        canHitUnderwater = false;
        canHitUnderground = false;
        canHitInMidAir = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CannotMiss;
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }
        if (p.Number == 720 && p.AdditionalData == "unbound")
        {
            return false;
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("But " + p.GetDisplayName() + " can't use the move!"));
            return true;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;

        if (own == true)
        {
            if (battleScreen.FieldEffects.DetectCounter.Opponent > 0 || battleScreen.FieldEffects.ProtectCounter.Opponent > 0 || battleScreen.FieldEffects.KingsShieldCounter.Opponent > 0 || battleScreen.FieldEffects.SpikyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.BanefulBunkerCounter.Opponent > 0 || battleScreen.FieldEffects.CraftyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.MatBlockCounter.Opponent > 0 || battleScreen.FieldEffects.WideGuardCounter.Opponent > 0 || battleScreen.FieldEffects.QuickGuardCounter.Opponent > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Hyperspace Fury lifted " + op.GetDisplayName() + "'s protection!"));
            }
            battleScreen.FieldEffects.DetectCounter.Opponent = 0;
            battleScreen.FieldEffects.ProtectCounter.Opponent = 0;
            battleScreen.FieldEffects.KingsShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.SpikyShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.BanefulBunkerCounter.Opponent = 0;
            battleScreen.FieldEffects.CraftyShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.MatBlockCounter.Opponent = 0;
            battleScreen.FieldEffects.WideGuardCounter.Opponent = 0;
            battleScreen.FieldEffects.QuickGuardCounter.Opponent = 0;
        }
        else
        {
            op = battleScreen.SelfPokemon;
            if (battleScreen.FieldEffects.DetectCounter.Self > 0 || battleScreen.FieldEffects.ProtectCounter.Self > 0 || battleScreen.FieldEffects.KingsShieldCounter.Self > 0 || battleScreen.FieldEffects.SpikyShieldCounter.Self > 0 || battleScreen.FieldEffects.BanefulBunkerCounter.Self > 0 || battleScreen.FieldEffects.CraftyShieldCounter.Self > 0 || battleScreen.FieldEffects.MatBlockCounter.Self > 0 || battleScreen.FieldEffects.WideGuardCounter.Self > 0 || battleScreen.FieldEffects.QuickGuardCounter.Self > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Hyperspace Fury lifted " + op.GetDisplayName() + "'s protection!"));
            }
            battleScreen.FieldEffects.DetectCounter.Self = 0;
            battleScreen.FieldEffects.ProtectCounter.Self = 0;
            battleScreen.FieldEffects.KingsShieldCounter.Self = 0;
            battleScreen.FieldEffects.SpikyShieldCounter.Self = 0;
            battleScreen.FieldEffects.BanefulBunkerCounter.Self = 0;
            battleScreen.FieldEffects.CraftyShieldCounter.Self = 0;
            battleScreen.FieldEffects.MatBlockCounter.Self = 0;
            battleScreen.FieldEffects.WideGuardCounter.Self = 0;
            battleScreen.FieldEffects.QuickGuardCounter.Self = 0;
        }

        battleScreen.Battle.LowerStat(own, own, battleScreen, "Defense", 1, "", "move:hyperspacefury");
    }

}
