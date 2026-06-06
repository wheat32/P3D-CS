using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class HiddenPower : Attack
{
    public HiddenPower()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 237;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 60;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Hidden Power");
        Description = "A unique attack that varies in type depending on the Pokémon using it.";
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

        if (p.Ability.Name.ToLower() == "normalize")
        {
            return new Element(Element.Types.Normal);
        }
        else
        {
            int a = GetLastBit(p.IVHP);
            int b = GetLastBit(p.IVAttack);
            int c = GetLastBit(p.IVDefense);
            int d = GetLastBit(p.IVSpeed);
            int e = GetLastBit(p.IVSpAttack);
            int f = GetLastBit(p.IVSpDefense);

            int t = (int)(Math.Floor((double)(((a + 2 * b + 4 * c + 8 * d + 16 * e + 32 * f) * 15) / 63)));

            switch (t)
            {
                case 0:
                    return new Element(Element.Types.Fighting);
                    break;
                case 1:
                    return new Element(Element.Types.Flying);
                    break;
                case 2:
                    return new Element(Element.Types.Poison);
                    break;
                case 3:
                    return new Element(Element.Types.Ground);
                    break;
                case 4:
                    return new Element(Element.Types.Rock);
                    break;
                case 5:
                    return new Element(Element.Types.Bug);
                    break;
                case 6:
                    return new Element(Element.Types.Ghost);
                    break;
                case 7:
                    return new Element(Element.Types.Steel);
                    break;
                case 8:
                    return new Element(Element.Types.Fire);
                    break;
                case 9:
                    return new Element(Element.Types.Water);
                    break;
                case 10:
                    return new Element(Element.Types.Grass);
                    break;
                case 11:
                    return new Element(Element.Types.Electric);
                    break;
                case 12:
                    return new Element(Element.Types.Psychic);
                    break;
                case 13:
                    return new Element(Element.Types.Ice);
                    break;
                case 14:
                    return new Element(Element.Types.Dragon);
                    break;
                case 15:
                    return new Element(Element.Types.Dark);
                    break;
            }
        }

        return type;
    }

    private int GetLastBit(int i)
    {
        return i % 2;
    }

}
