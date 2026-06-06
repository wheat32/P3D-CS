using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Present : Attack
{
    public Present()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 217;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Present");
        Description = "The user attacks by giving the target a gift with a hidden trap. It restores HP sometimes, however.";
        criticalChance = 1;
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
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
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

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < 80)
        {
            return false;
        }
        else
        {
            Pokemon op = battleScreen.OpponentPokemon;
            if (own == false)
            {
                op = battleScreen.SelfPokemon;
            }

            if (op.HP < op.MaxHP && op.HP > 0)
            {
                battleScreen.Battle.GainHP((int)(Math.Ceiling((double)(op.MaxHP / 4))), own == false, own, battleScreen, op.GetDisplayName() + " had its HP restored!", "move:present");
            }

            return true;
        }
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int r = Core.Random.Next(0, 80);
        if (r < 40)
        {
            return 40;
        }
        else if (r >= 40 && r < 70)
        {
            return 80;
        }
        else if (r >= 70)
        {
            return 120;
        }

        return 40;
    }

}
