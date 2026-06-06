using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class TechnoBlast : Attack
{
    public TechnoBlast()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 546;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 120;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Techno Blast");
        Description = "The user fires a beam of light at its target. The move's type changes depending on the Drive the user holds.";
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

    public override Element GetAttackType(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int itemID = 0;
        if (p.Item != null)
        {
            if (p.Item.IsGameModeItem == false)
            {
                itemID = p.Item.ID;
            }
            else
            {
                return new Element(Element.Types.Normal);
            }
        }

        switch (itemID)
        {
            case 1996:
                return new Element(Element.Types.Fire);
                break;
            case 1997:
                return new Element(Element.Types.Ice);
                break;
            case 1998:
                return new Element(Element.Types.Water);
                break;
            case 1999:
                return new Element(Element.Types.Electric);
                break;
            default:
                return new Element(Element.Types.Normal);
                break;
        }

        return type;
    }

}
