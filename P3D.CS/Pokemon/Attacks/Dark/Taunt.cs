using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class Taunt : Attack
{
    public Taunt()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 269;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Taunt");
        Description = "The target is taunted into a rage that allows it to use only attack moves for three turns.";
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

        if (op.Ability.Name.ToLower() == "oblivious" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
        else
        {
            if (own == true)
            {

                if (battleScreen.FieldEffects.Taunt.Opponent == 0)
                {
                    if (battleScreen.OpponentPokemon.Ability.Name.ToLower() != "aroma veil")
                    {
                        battleScreen.FieldEffects.Taunt.Opponent = 3;
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " fell for the Taunt."));
                    }
                    else
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject("Aroma Veil protected " + battleScreen.OpponentPokemon.GetDisplayName() + " from " + Name + "!"));
                    }
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                }
            }
            else
            {
                if (battleScreen.FieldEffects.Taunt.Self == 0)
                {
                    if (battleScreen.SelfPokemon.Ability.Name.ToLower() != "aroma veil")
                    {
                        battleScreen.FieldEffects.Taunt.Self = 3;
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " fell for the Taunt."));
                    }
                    else
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject("Aroma Veil protected " + battleScreen.SelfPokemon.GetDisplayName() + " from " + Name + "!"));
                    }
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                }
            }
        }
    }

}
