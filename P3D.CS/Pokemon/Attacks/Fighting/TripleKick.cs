using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class TripleKick : Attack
{
    public TripleKick()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 167;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 10;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Triple Kick");
        Description = "A consecutive three-kick attack that becomes more powerful with each successive hit.";
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
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int TripleKick = 10;
        switch (battleScreen.FieldEffects.TempTripleKick)
        {
            case 1:
                TripleKick = 20;
                break;
            case 2:
                TripleKick = 30;
                break;
        }
        battleScreen.FieldEffects.TempTripleKick += 1;
        return TripleKick;
    }

    public override int GetTimesToAttack(bool own, BattleScreen battleScreen)
    {
        int r = Core.Random.Next(0, 100);
        if (r < 73)
        {
            return 3;
        }
        else if (r >= 73 && r < 92)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

}
