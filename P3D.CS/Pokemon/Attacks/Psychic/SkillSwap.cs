using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class SkillSwap : Attack
{
    public SkillSwap()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 285;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Skill Swap");
        Description = "The user employs its psychic power to exchange Abilities with the target.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

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
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        String[] bannedAbilities = {"wonder guard", "multitype", "illusion", "stance change"};

        if (bannedAbilities.Contains(p.Ability.Name.ToLower()) == false && bannedAbilities.Contains(op.Ability.Name.ToLower()) == false)
        {
            int pAbility = p.Ability.ID;
            int opAbility = op.Ability.ID;

            p.Ability = Ability.GetAbilityByID(opAbility);
            op.Ability = Ability.GetAbilityByID(pAbility);

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " swapped abilities with its target!"));
        }
        else
        {
            // fails
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
