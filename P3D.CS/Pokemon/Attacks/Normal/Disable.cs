using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Disable : Attack
{
    public Disable()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 50;
        originalPP = 20;
        currentPP = 20;
        maxPP = 32;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Disable");
        Description = "For four turns, the target will be unable to use whichever move it last used.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllAdjacentTargets;
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
        isSoundMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.LowerAttack;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon Target = battleScreen.OpponentPokemon;
        Attack LastMove = battleScreen.FieldEffects.LastMove.Opponent;
        if (own == false)
        {
            Target = battleScreen.SelfPokemon;
            LastMove = battleScreen.FieldEffects.LastMove.Self;
        }
        if (LastMove != null)
        {
            if ("struggle".Equals(LastMove.Name.ToLower()) == false && LastMove.Disabled == 0)
            {
                bool hasDisabledMove = false;
                int TargetMoveIndex = -1;
                for (int a = 0; a <= Target.Attacks.Count - 1; a++)
                {
                    if (Target.Attacks[a].Disabled > 0)
                    {
                        hasDisabledMove = true;
                        break;
                    }
                    if (Target.Attacks[a].ID == LastMove.ID)
                    {
                        TargetMoveIndex = a;
                    }
                }
                if (TargetMoveIndex != -1 && hasDisabledMove == false)
                {
                    Target.Attacks[TargetMoveIndex].Disabled = 4;
                    battleScreen.BattleQuery.Add(new TextQueryObject(Target.GetDisplayName() + "'s " + Target.Attacks[TargetMoveIndex].Name + " was Disabled!"));
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
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }

    }

}
