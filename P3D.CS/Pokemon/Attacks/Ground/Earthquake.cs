using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class Earthquake : Attack
{
    public Earthquake()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 89;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 100;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Earthquake");
        Description = "The user sets off an earthquake that strikes those around it.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentTargets;
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
        canHitUnderground = true;
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int dig = battleScreen.FieldEffects.DigCounter.Opponent;
        if (own == false)
        {
            dig = battleScreen.FieldEffects.DigCounter.Self;
        }

        if (dig > 0)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip, false);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Ground\Earthquake", 0.0F, 0);
        MoveAnimation.AnimationCameraOscillateMove(new Vector3(0, 0, 0.075f), 0.045f, true, 16, 0, 0, 0, new Vector3(0, 0, 1));

        MoveAnimation.AnimationCameraOscillateMove(new Vector3(0, 0.06f, 0), 0.03f, true, 4, 0, 0, 0, new Vector3(0, 0, 1));
        MoveAnimation.AnimationCameraOscillateMove(new Vector3(0, -0.06f, 0), 0.03f, true, 4, 4, 0, 0, new Vector3(0, 0, 1));
        MoveAnimation.AnimationCameraOscillateMove(new Vector3(0, 0.06f, 0), 0.03f, true, 4, 8, 0, 0, new Vector3(0, 0, 1));
        MoveAnimation.AnimationCameraOscillateMove(new Vector3(0, -0.06f, 0), 0.03f, true, 4, 12, 0, 0, new Vector3(0, 0, 1));

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
