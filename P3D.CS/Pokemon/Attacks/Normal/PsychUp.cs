using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class PsychUp : Attack
{
    public PsychUp()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 244;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Psych Up");
        Description = "The user hypnotizes itself into copying any stat change made by the target.";
        criticalChance = 0;
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
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        p.StatAttack = op.StatAttack;
        p.StatDefense = op.StatDefense;
        p.StatSpAttack = op.StatSpAttack;
        p.StatSpDefense = op.StatSpDefense;
        p.StatSpeed = op.StatSpeed;
        p.Evasion = op.Evasion;
        p.Accuracy = op.Accuracy;

        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " copied " + op.GetDisplayName() + "'s stat changes!"));
    }

}
