using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class HeartSwap : Attack
{
    public HeartSwap()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 391;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Heart Swap");
        Description = "Heart Swap switches any increase and/or decrease in stat changes with the target.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
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


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.RaiseAttack;
        aiField2 = AIField.RaiseSpAttack;
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

        int recvattack = op.StatAttack;
        int recvdefense = op.StatDefense;
        int recvspattack = op.StatSpAttack;
        int recvspdefense = op.StatSpDefense;
        int recvspeed = op.StatSpeed;
        int recvaccuracy = op.Accuracy;
        int recvevasion = op.Evasion;

        int userattack = p.StatAttack;
        int userdefense = p.StatDefense;
        int userspattack = p.StatSpAttack;
        int userspdefense = p.StatSpDefense;
        int userspeed = p.StatSpeed;
        int useraccuracy = p.Accuracy;
        int userevasion = p.Evasion;

        p.StatAttack = recvattack;
        p.StatDefense = recvdefense;
        p.StatSpAttack = recvspattack;
        p.StatSpDefense = recvspdefense;
        p.StatSpeed = recvspeed;
        p.Accuracy = recvaccuracy;
        p.Evasion = recvevasion;

        op.StatAttack = userattack;
        op.StatDefense = userdefense;
        op.StatSpAttack = userspattack;
        op.StatSpDefense = userspdefense;
        op.StatSpeed = userspeed;
        op.Accuracy = useraccuracy;
        op.Evasion = userevasion;

        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "switched stat changes with the target!"));

    }

}
