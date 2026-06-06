using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class LastResort : Attack
{
    public LastResort()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 387;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 140;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Last Resort");
        Description = "This move can be used only after the user has used all the other moves it knows in the battle.";
        criticalChance = 1;
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
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;
        // #End
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        List<int> moveIDs = [];
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        foreach (Attack Attack in p.Attacks)
        {
            if (moveIDs.Contains(Attack.ID) == false)
            {
                moveIDs.Add(Attack.ID);
            }
        }

        bool usedMoves = true;
        List<int> AllUsedMoves = battleScreen.FieldEffects.UsedMoves.Self;
        if (own == false)
        {
            AllUsedMoves = battleScreen.FieldEffects.UsedMoves.Opponent;
        }

        foreach (int moveID in moveIDs)
        {
            if (AllUsedMoves.Contains(moveID) == false && moveID != 387)
            {
                usedMoves = false;
                break;
            }
        }

        if (usedMoves == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }
        else
        {
            return false;
        }
    }

}
