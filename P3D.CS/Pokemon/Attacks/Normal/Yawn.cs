using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Yawn : Attack
{
    public Yawn()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 281;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Yawn");
        Description = "The user lets loose a huge yawn that lulls the target into falling asleep on the next turn.";
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
        immunityAffected = true;
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

        aiField1 = AIField.Sleep;
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

        if (((op.Ability.Name.ToLower() == "insomnia" || op.Ability.Name.ToLower() == "vital spirit" || op.Ability.Name.ToLower() == "sweet veil") == true && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true) == true || op.Status == Pokemon.StatusProblems.Sleep)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
        else
        {
            if (own == true)
            {
                if (battleScreen.FieldEffects.Yawn.Opponent == 0)
                {
                    battleScreen.FieldEffects.Yawn.Opponent = 2;
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " grew drowsy."));
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                }
            }
            else
            {
                if (battleScreen.FieldEffects.Yawn.Self == 0)
                {
                    battleScreen.FieldEffects.Yawn.Self = 2;
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " grew drowsy."));
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                }
            }
        }
    }

}
