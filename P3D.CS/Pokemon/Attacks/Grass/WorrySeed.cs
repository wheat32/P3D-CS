using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class WorrySeed : Attack
{
    public WorrySeed()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 388;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Worry Seed");
        Description = "A seed that causes worry is planted on the target. It prevents sleep by making the target's Ability Insomnia.";
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
        String[] bannedAbilities = {"insomnia", "truant", "multitype", "stance change", "schooling", "comatose", "shields down", "disguise", "rks system", "battle bond"};
        if (bannedAbilities.Contains(op.Ability.Name.ToLower()) == false)
        {
            op.Ability = Ability.GetAbilityByID(15);
            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " acquired Insomnia!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
