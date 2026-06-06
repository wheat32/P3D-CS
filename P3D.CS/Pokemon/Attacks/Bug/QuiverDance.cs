using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Bug;

public class QuiverDance : Attack
{
    public QuiverDance()
    {
        // #Definitions
        type = new Element(Element.Types.Bug);
        ID = 483;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Quiver Dance");
        Description = "The user lightly performs a beautiful, mystic dance. This boosts the user's Sp. Atk, Sp. Def, and Speed stats.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;

        isDanceMove = true;

        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.RaiseSpAttack;
        aiField2 = AIField.RaiseSpDefense;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Attack", 1, "", "move:quiverdance");
        bool c = battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Defense", 1, "", "move:quiverdance");
        bool d = battleScreen.Battle.RaiseStat(own, own, battleScreen, "Speed", 1, "", "move:quiverdance");
        if (b == false && c == false && d == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
