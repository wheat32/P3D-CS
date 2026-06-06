using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class HealingWish : Attack
{
    public HealingWish()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 361;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Healing Wish");
        Description = "The user faints. In return, the Pokemon taking its place will have its HP restored and status conditions cured.";
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

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        battleScreen.Battle.ReduceHP(p.HP, own, own, battleScreen, "", "move:healingwish");
        battleScreen.Battle.FaintPokemon(own, own, battleScreen, "", "move:healingwish");

        if (own == true)
        {
            battleScreen.FieldEffects.HealingWish.Self = true;
        }
        else
        {
            battleScreen.FieldEffects.HealingWish.Opponent = true;
        }
    }

}
