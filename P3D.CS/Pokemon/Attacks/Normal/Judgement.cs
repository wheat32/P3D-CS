using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Judgement : Attack
{
    public Judgement()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 449;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 100;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Judgement");
        Description = "The user releases countless shots of light at the target. This move's type varies depending on the kind of Plate the user is holding.";
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
            case 267:
                return new Element(Element.Types.Dragon);
                break;
            case 268:
                return new Element(Element.Types.Dark);
                break;
            case 269:
                return new Element(Element.Types.Ground);
                break;
            case 270:
                return new Element(Element.Types.Fighting);
                break;
            case 271:
                return new Element(Element.Types.Fire);
                break;
            case 272:
                return new Element(Element.Types.Ice);
                break;
            case 273:
                return new Element(Element.Types.Bug);
                break;
            case 274:
                return new Element(Element.Types.Steel);
                break;
            case 275:
                return new Element(Element.Types.Grass);
                break;
            case 276:
                return new Element(Element.Types.Psychic);
                break;
            case 277:
                return new Element(Element.Types.Fairy);
                break;
            case 278:
                return new Element(Element.Types.Flying);
                break;
            case 279:
                return new Element(Element.Types.Water);
                break;
            case 280:
                return new Element(Element.Types.Ghost);
                break;
            case 281:
                return new Element(Element.Types.Rock);
                break;
            case 282:
                return new Element(Element.Types.Poison);
                break;
            case 283:
                return new Element(Element.Types.Electric);
                break;
            default:
                return new Element(Element.Types.Normal);
                break;
        }

        return type;
    }

}
