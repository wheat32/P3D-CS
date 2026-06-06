using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class GrassyTerrain : Attack
{
    public GrassyTerrain()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 580;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Grassy Terrain");
        Description = "The user turns the ground to grass for five turns. This restores the HP of Pokémon on the ground a little every turn and powers up Grass type-moves.";
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
        if (battleScreen.FieldEffects.GrassyTerrain <= 0)
        {

            battleScreen.FieldEffects.ElectricTerrain = 0;
            battleScreen.FieldEffects.GrassyTerrain = turns;
            battleScreen.FieldEffects.PsychicTerrain = 0;
            battleScreen.FieldEffects.MistyTerrain = 0;

            battleScreen.BattleQuery.Add(new TextQueryObject("Grass grew to cover the battlefield!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
