using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dragon;

public class Twister : Attack
{
    public Twister()
    {
        // #Definitions
        type = new Element(Element.Types.Dragon);
        ID = 239;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 40;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Twister");
        Description = "The user whips up a vicious tornado to tear at the opposing team. It may also make targets flinch.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isWindMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        canHitInMidAir = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanFlinch;

        effectChances.Add(20);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:twister");
        }
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int fly = 0;
        int bounce = 0;

        if (own == true)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
            bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
        }
        else
        {
            fly = battleScreen.FieldEffects.FlyCounter.Self;
            bounce = battleScreen.FieldEffects.BounceCounter.Self;
        }

        if (fly > 0 || bounce > 0)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

}
