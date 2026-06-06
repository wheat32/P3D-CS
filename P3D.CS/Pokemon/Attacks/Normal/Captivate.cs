using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Captivate : Attack
{
    public Captivate()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 445;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Captivate");
        Description = "If any opposing Pokemon is the opposite gender of the user, it is charmed, which harshly lowers its Sp. Atk. stat.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
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

        aiField1 = AIField.LowerAttack;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
            p = battleScreen.OpponentPokemon;
        }

        if (op.Ability.Name.ToLower() == "oblivious" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
        else
        {
            if (op.Gender != Pokemon.Genders.Genderless && op.Gender != p.Gender)
            {
                if (battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Attack", 1, "", "move:captivate") == false)
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

}
