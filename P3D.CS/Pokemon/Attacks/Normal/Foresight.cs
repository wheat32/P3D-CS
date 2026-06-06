using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Foresight : Attack
{
    public Foresight()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 193;
        originalPP = 40;
        currentPP = 40;
        maxPP = 40;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Foresight");
        Description = "Enables a Ghost-type target to be hit by Normal- and Fighting-type attacks. It also enables an evasive target to be hit.";
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        op.Evasion = 0;

        if (own == true)
        {
            battleScreen.FieldEffects.Foresight.Opponent = 1;
        }
        else
        {
            battleScreen.FieldEffects.Foresight.Self = 1;
        }

        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was identified!"));
    }

}
