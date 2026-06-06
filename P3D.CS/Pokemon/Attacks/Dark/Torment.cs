using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class Torment : Attack
{
    public Torment()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 259;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Torment");
        Description = "The user torments and enrages the target, making it incapable of using the same move twice in a row.";
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
        int torment = battleScreen.FieldEffects.Torment.Opponent;
        if (own == false)
        {
            torment = battleScreen.FieldEffects.Torment.Self;
        }

        if (torment == 0)
        {
            if (own == true)
            {
                if (battleScreen.OpponentPokemon.Ability.Name.ToLower() != "aroma veil")
                {
                    battleScreen.FieldEffects.Torment.Opponent = 1;
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was subjected to " + Name + "!"));
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject("Aroma Veil protected " + battleScreen.OpponentPokemon.GetDisplayName() + " from " + Name + "!"));
                }
            }
            else
            {
                if (battleScreen.SelfPokemon.Ability.Name.ToLower() != "aroma veil")
                {
                    battleScreen.FieldEffects.Torment.Self = 1;
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was subjected to " + Name + "!"));
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject("Aroma Veil protected " + battleScreen.SelfPokemon.GetDisplayName() + " from " + Name + "!"));
                }
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
