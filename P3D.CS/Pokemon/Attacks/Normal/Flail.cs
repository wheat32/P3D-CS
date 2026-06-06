using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Flail : Attack
{
    public Flail()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 175;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Flail");
        Description = "The user flails about aimlessly to attack. It becomes more powerful the less HP the user has.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
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

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon ownP = battleScreen.SelfPokemon;
        if (own == false)
        {
            ownP = battleScreen.OpponentPokemon;
        }

        double P = (48 * ownP.HP) / ownP.MaxHP;

        if (P > 32)
        {
            return 20;
        }
        else if (P <= 32 && P >= 17)
        {
            return 40;
        }
        else if (P <= 16 && P >= 10)
        {
            return 80;
        }
        else if (P <= 9 && P >= 5)
        {
            return 100;
        }
        else if (P <= 4 && P >= 2)
        {
            return 150;
        }
        else if (P <= 1)
        {
            return 200;
        }

        return 20;
    }

}
