using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class Defog : Attack
{
    public Defog()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 432;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Defog");
        Description = "A strong wind blows away the target's barriers such as Reflect or Light Screen. This also lowers the target's evasiveness.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
        snatchAffected = true;
        mirrorMoveAffected = true;
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Evasion", 1, "", "move:defog") == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }

            battleScreen.FieldEffects.Safeguard.Opponent = 0;
            battleScreen.FieldEffects.Mist.Opponent = 0;
            battleScreen.FieldEffects.LightScreen.Opponent = 0;
            battleScreen.FieldEffects.Reflect.Opponent = 0;
            battleScreen.FieldEffects.Spikes.Opponent = 0;
            battleScreen.FieldEffects.StealthRock.Opponent = 0;
            battleScreen.FieldEffects.ToxicSpikes.Opponent = 0;
            battleScreen.FieldEffects.Safeguard.Self = 0;
            battleScreen.FieldEffects.LightScreen.Self = 0;
            battleScreen.FieldEffects.Mist.Self = 0;
            battleScreen.FieldEffects.Reflect.Self = 0;
            battleScreen.FieldEffects.Spikes.Self = 0;
            battleScreen.FieldEffects.StealthRock.Self = 0;
            battleScreen.FieldEffects.ToxicSpikes.Self = 0;
            battleScreen.FieldEffects.ElectricTerrain = 0;
            battleScreen.FieldEffects.PsychicTerrain = 0;
            battleScreen.FieldEffects.MistyTerrain = 0;
            battleScreen.FieldEffects.GrassyTerrain = 0;

    }

}
