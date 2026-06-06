using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class GuardSwap : Attack
{
    public GuardSwap()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 385;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Guard Swap");
        Description = "The user employs its psychic power to switch changes to its Defense and Sp. Def. stats with the target.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
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

        int userdefense = p.StatDefense;
        int recvdefense = op.StatDefense;
        int userspdefense = p.StatSpDefense;
        int recvspdefense = op.StatSpDefense;

        p.StatDefense = recvdefense;
        op.StatDefense = userdefense;
        p.StatSpDefense = recvspdefense;
        op.StatSpDefense = userspdefense;

        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " switched all changes to its Defense && Sp. Def. with the target!"));

    }

}
