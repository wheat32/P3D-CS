using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class OminousWind : Attack
{
    public OminousWind()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 466;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 60;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Ominous Wind");
        Description = "The user blasts the target with a gust of repulsive wind. It may also raise all the user's stats at once.";
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
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isWindMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanRaiseAttack;
        aiField3 = AIField.CanRaiseSpAttack;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Attack", 1, "", "move:ominouswind");
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Defense", 1, "", "move:ominouswind");
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Attack", 1, "", "move:ominouswind");
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Defense", 1, "", "move:ominouswind");
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Speed", 1, "", "move:ominouswind");
        }
    }

}
