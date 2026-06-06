using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class NaturePower : Attack
{
    public NaturePower()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 267;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Nature Power");
        Description = "An attack that makes use of nature’s power. Its effects vary depending on the user’s environment.";
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
    }

    public static int GetMoveID(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.ElectricTerrain > 0)
        {
            return 85;
        }
        else if (battleScreen.FieldEffects.GrassyTerrain > 0)
        {
            return 412;
        }
        else if (battleScreen.FieldEffects.MistyTerrain > 0)
        {
            return 585;
        }
        else if (battleScreen.FieldEffects.PsychicTerrain > 0)
        {
            return 94;
        }
        else
        {
            switch (Screen.Level.Terrain.TerrainType)
            {
                case Terrain.TerrainTypes.Plain:
                    return 161;
                    break;
                case Terrain.TerrainTypes.Cave:
                    return 247;
                    break;
                case Terrain.TerrainTypes.DistortionWorld:
                    return 185;
                    break;
                case Terrain.TerrainTypes.LongGrass:
                    return 75;
                    break;
                case Terrain.TerrainTypes.Magma:
                    return 172;
                    break;
                case Terrain.TerrainTypes.PondWater:
                    return 61;
                    break;
                case Terrain.TerrainTypes.Puddles:
                    return 426;
                    break;
                case Terrain.TerrainTypes.Rock:
                    return 157;
                    break;
                case Terrain.TerrainTypes.Sand:
                    return 89;
                    break;
                case Terrain.TerrainTypes.SeaWater:
                    return 56;
                    break;
                case Terrain.TerrainTypes.Snow:
                    return 58;
                    break;
                case Terrain.TerrainTypes.TallGrass:
                    return 402;
                    break;
                case Terrain.TerrainTypes.Underwater:
                    return 291;
                    break;
            }
            return 89;
        }
    }

}
