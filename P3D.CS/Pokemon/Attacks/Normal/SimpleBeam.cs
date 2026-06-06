using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class SimpleBeam : Attack
{
    public SimpleBeam()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 493;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Simple Beam");
        Description = "The user's mysterious psychic wave changes the target's Ability to Simple.";
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
        isHealingMove = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isRecoilMove = false;

        immunityAffected = true;
        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }
        String[] bannedAbilities = {"simple", "truant", "multitype", "stance change", "schooling", "comatose", "shields down", "disguise", "rks system", "battle bond"};
        if (bannedAbilities.Contains(op.Ability.Name.ToLower()) == false)
        {
            op.Ability = Ability.GetAbilityByID(86);
            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " acquired Simple!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
