using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class CosmicPower : Attack
{
    public CosmicPower()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 322;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Cosmic Power");
        Description = "The user absorbs a mystical power from space to raise its Defense and Sp. Def. stats.";
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
        snatchAffected = true;
        mirrorMoveAffected = true;
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

        aiField1 = AIField.RaiseDefense;
        aiField2 = AIField.RaiseSpDefense;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Defense", 1, "", "move:cosmicpower");
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Defense", 1, "", "move:cosmicpower");
    }

}
