using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class ElectricTerrain : Attack
{
    public ElectricTerrain()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 604;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Electric Terrain");
        Description = "The user electrifies the ground for five turns, powering up Electric-type moves. Pokémon on the ground no longer fall asleep.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.All;
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int turns = BattleCalculation.FieldEffectTurns(battleScreen, own, Name.ToLower());
        if (battleScreen.FieldEffects.ElectricTerrain <= 0)
        {

            battleScreen.FieldEffects.ElectricTerrain = turns;
            battleScreen.FieldEffects.GrassyTerrain = 0;
            battleScreen.FieldEffects.PsychicTerrain = 0;
            battleScreen.FieldEffects.MistyTerrain = 0;

            battleScreen.BattleQuery.Add(new TextQueryObject("An electric current runs across the battlefield!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
