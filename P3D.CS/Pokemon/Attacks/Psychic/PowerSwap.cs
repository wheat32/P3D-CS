using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class PowerSwap : Attack
{
    public PowerSwap()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 384;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Power Swap");
        Description = "The user employs its psychic power to switch changes to its Attack and Sp. Atk. stats with the target.";
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

        int userattack = p.StatAttack;
        int recvattack = op.StatAttack;
        int userspattack = p.StatSpAttack;
        int recvspattack = op.StatSpAttack;

        p.StatAttack = recvattack;
        op.StatAttack = userattack;
        p.StatSpAttack = recvspattack;
        op.StatSpAttack = userspattack;

        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " switched all changes to its Attack && Sp. Atk. with the target!"));

    }

}
