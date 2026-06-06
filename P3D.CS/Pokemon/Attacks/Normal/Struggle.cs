using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Struggle : Attack
{
    public Struggle()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 165;
        originalPP = 1;
        currentPP = 1;
        maxPP = 1;
        Power = 50;
        Accuracy = 0;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Struggle");
        Description = "An attack that is used in desperation only if the user has no PP. It also hurts the user slightly.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        canGainSTAB = false;
        useAccEvasion = false;
        // #End
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        return false;
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " has no usable attacks left!"));
        }
    }

    public override void PreAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.OpponentPokemon;
        if (own == false)
        {
            battleScreen.Battle.ChangeCameraAngle(1, false, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " has no usable attacks left!"));
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int recoilDamage = (int)(Math.Ceiling((double)(p.MaxHP / 4)));
        if (recoilDamage <= 0)
        {
            recoilDamage = 1;
        }

        battleScreen.Battle.ReduceHP(recoilDamage, own, own, battleScreen, p.GetDisplayName() + " is damaged by recoil!", "move:struggle");
    }

}
