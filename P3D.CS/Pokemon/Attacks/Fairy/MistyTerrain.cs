using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fairy;

public class MistyTerrain : Attack
{
    public MistyTerrain()
    {
        // #Definitions
        type = new Element(Element.Types.Fairy);
        ID = 581;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Misty Terrain");
        Description = "This protects Pokémon on the ground from status conditions and halves damage from Dragon-type moves for five turns.";
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
        if (battleScreen.FieldEffects.MistyTerrain <= 0)
        {

            battleScreen.FieldEffects.ElectricTerrain = 0;
            battleScreen.FieldEffects.GrassyTerrain = 0;
            battleScreen.FieldEffects.PsychicTerrain = 0;
            battleScreen.FieldEffects.MistyTerrain = turns;

            battleScreen.BattleQuery.Add(new TextQueryObject("Mist swirls around the battlefield!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
