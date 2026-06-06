using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class Rest : Attack
{
    public Rest()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 156;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Rest");
        Description = "The user goes to sleep for two turns. It fully restores the users HP and heals any status problem.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = true;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = true;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Healing;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool fails = false;
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }
        StatusProblems status = p.Status;

        int healBlock = battleScreen.FieldEffects.HealBlock.Self;
        if (own == false)
        {
            healBlock = battleScreen.FieldEffects.HealBlock.Opponent;
        }

        if (healBlock > 0)
        {
            fails = true;
        }
        else
        {
            if (p.HP >= p.MaxHP)
            {
                fails = true;
            }
            else
            {
                if (battleScreen.Battle.InflictSleep(own, own, battleScreen, 3, "", "move:rest") == false)
                {
                    fails = true;
                }
            }
        }

        if (fails == true)
        {
            p.Status = status;
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " slept && became healthy."));
            battleScreen.Battle.GainHP(p.MaxHP - p.HP, own, own, battleScreen, "", "move:rest");
        }
    }

}
