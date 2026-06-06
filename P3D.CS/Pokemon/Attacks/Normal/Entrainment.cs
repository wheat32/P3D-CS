using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Entrainment : Attack
{
    public Entrainment()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 494;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Entrainment");
        Description = "The user dances with an odd rhythm that compels the target to mimic it, making the target's Ability the same as the user's.";
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
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        String[] bannedAbilitiesOpp = {"simple", "truant", "multitype", "stance change", "schooling", "comatose", "shields down", "disguise", "rks system", "battle bond"};
        String[] bannedAbilitiesOwn = {"trace", "forecast", "flower gift", "zen mode", "illusion", "imposter", "power of alchemy", "receiver", "disguise", "power construct"};

        if (bannedAbilitiesOpp.Contains(op.Ability.Name.ToLower()) == false && bannedAbilitiesOwn.Contains(p.Ability.Name.ToLower()) == false)
        {
            op.Ability = Ability.GetAbilityByID(p.Ability.ID);
            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " acquired " + Localization.GetString("ability_name_" + op.Ability.ID.ToString(), op.Ability.Name) + "!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
