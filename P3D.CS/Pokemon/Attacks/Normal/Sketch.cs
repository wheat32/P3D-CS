using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Sketch : Attack
{
    public Sketch()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 166;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Sketch");
        Description = "It enables the user to permanently learn the move last used by the target. Once used, Sketch disappears.";
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
        mirrorMoveAffected = false;
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
        Attack lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Self;
        }

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.IsTransformed == false)
        {
            if (lastMove != null)
            {
                Attack newAttack = BattleSystem.Attack.GetAttackByID(lastMove.ID);

                foreach (Attack a in p.Attacks)
                {
                    if (a.ID == 166)
                    {
                        p.Attacks.Remove[a];
                        break;
                    }
                }

                p.Attacks.Add(newAttack);

                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " learned " + newAttack.Name + "!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
