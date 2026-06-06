using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class BurnUp : Attack
{
    public BurnUp()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 682;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 130;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Burn Up");
        Description = "To inflict massive damage, the user burns itself out. After using this move, the user will no longer be Fire type.";
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
        kingsrockAffected = true;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = true;

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
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.IsType(Element.Types.Fire) == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        p.OriginalType1 = new Element(p.Type1.Type);
        p.OriginalType2 = new Element(p.Type2.Type);

        if (p.Type2.Type == Element.Types.Blank)
        {
            // Pure fire

            p.Type1.Type = Element.Types.Blank;
        }
        else
        {
            // One of the types is fire

            if (p.Type1.Type == Element.Types.Fire)
            {
                p.Type1.Type = p.Type2.Type;
            }
            p.Type2.Type = Element.Types.Blank;
        }

    }

}
