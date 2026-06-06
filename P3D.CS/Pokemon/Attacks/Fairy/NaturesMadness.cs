using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fairy;

public class NaturesMadness : Attack
{
    public NaturesMadness()
    {
        // #Definitions
        type = new Element(Element.Types.Fairy);
        ID = 717;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 90;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Nature's Madness");
        Description = "The user hits the target with the force of nature. It halves the target's HP.";
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
        kingsrockAffected = true;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override int GetDamage(bool critical, bool own, bool targetPokemon, BattleScreen battleScreen, String extraParameter = "", Attack typeEffectivenessAttack = null)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        int damage = (int)(Math.Floor((double)(op.HP / 2)));

        if (damage <= 0)
        {
            damage = 1;
        }

        return damage;
    }

}
