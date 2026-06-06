using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Metronome : Attack
{
    public Metronome()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 118;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Metronome");
        Description = "The user waggles a finger and stimulates its brain into randomly using nearly any move.";
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
        mirrorMoveAffected = false;
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


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End
    }

    public static Attack GetMetronomeMove()
    {
        int moveID = Core.Random.Next(1, MOVE_COUNT + 1);

        List<int> forbiddenIDs = [68, 102, 118, 119, 144, 165, 166, 168, 173, 182, 194, 197, 203, 214, 243, 264, 266, 267, 270, 271, 274, 289, 343, 364, 382, 383, 415, 448, 476, 469, 495, 501, 511, 516, 546, 547, 548, 553, 554, 555, 557];

        while (forbiddenIDs.Contains(moveID) == true)
        {
            moveID = Core.Random.Next(1, MOVE_COUNT + 1);
        }

        return Attack.GetAttackByID(moveID);
    }

}
