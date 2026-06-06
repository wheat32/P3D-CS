using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class Curse : Attack
{
    public Curse()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 174;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Curse");
        Description = "A move that works differently for the Ghost type than for all other types.";
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
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.RaiseAttack;
        aiField2 = AIField.RaiseDefense;
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

        bool isGhost = false;
        if (p.Type1.Type == Element.Types.Ghost || p.Type2.Type == Element.Types.Ghost)
        {
            isGhost = true;
        }

        if (isGhost == true)
        {
            bool cursed = false;
            if (own == true)
            {
                if (battleScreen.FieldEffects.Curse.Opponent > 0)
                {
                    cursed = true;
                }
            }
            else
            {
                if (battleScreen.FieldEffects.Curse.Self > 0)
                {
                    cursed = true;
                }
            }
            if (cursed == true)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
            else
            {
                if (own == true)
                {
                    battleScreen.FieldEffects.Curse.Opponent = 1;
                }
                else
                {
                    battleScreen.FieldEffects.Curse.Self = 1;
                }

                battleScreen.Battle.ReduceHP((int)(p.MaxHP / 2), own, own, battleScreen, p.GetDisplayName() + " cut its own HP && laid a curse on " + op.GetDisplayName() + "!", "move:curse");
            }
        }
        else
        {
            bool failed = false;

            if (battleScreen.Battle.LowerStat(own, own, battleScreen, "Speed", 1, "", "move:curse") == false)
            {
                failed = true;
            }

            if (failed == false)
            {
                battleScreen.Battle.RaiseStat(own, own, battleScreen, "Attack", 1, "", "move:curse");
                battleScreen.Battle.RaiseStat(own, own, battleScreen, "Defense", 1, "", "move:curse");
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
