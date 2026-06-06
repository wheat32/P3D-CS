using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class TarShot : Attack
{
    public TarShot()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 749;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Tar Shot");
        Description = "The user pours sticky tar over the target, lowering the target's Speed stat. The target becomes weaker to Fire-type moves.";
        criticalChance = 1;
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
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanLowerSpeed;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Speed", 1, "", "move:tarshot");


            if (own == true)
            {
                if (battleScreen.FieldEffects.TarShot.Opponent == false)
                {
                    battleScreen.FieldEffects.TarShot.Opponent = true;
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetName + " " + "became weaker to fire!"));
                }
            }
            else
            {
                if (battleScreen.FieldEffects.TarShot.Self == false)
                {
                    battleScreen.FieldEffects.TarShot.Self = true;
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetName + " " + "became weaker to fire!"));
                }
            }

    }

}
