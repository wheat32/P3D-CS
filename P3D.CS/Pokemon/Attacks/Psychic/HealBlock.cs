using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class HealBlock : Attack
{
    public HealBlock()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 377;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Heal Block");
        Description = "For five turns, the user prevents the opposing team from using any moves, Abilities, or held items that recover HP.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = true;
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
        if (own == true)
        {
            battleScreen.FieldEffects.HealBlock.Self = 5;
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was prevented from healing!"));
        }
        else
        {
            battleScreen.FieldEffects.HealBlock.Opponent = 5;
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was prevented from healing!"));
        }
    }

}
