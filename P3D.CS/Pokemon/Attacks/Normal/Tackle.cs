using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Tackle : Attack
{
    public Tackle()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 33;
        originalPP = 35;
        currentPP = 35;
        maxPP = 35;
        Power = 40;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Tackle");
        Description = "A physical attack in which the user charges and slams into the target with its whole body.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
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
        // #End
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationMove(null, false, -0.5F, 0, 0, 0.035F, false, false, 0, 0);
        MoveAnimation.AnimationMove(null, false, 0.5F, 0, 0, 0.06F, false, false, 1.25, 0);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Tackle", 0, 0);
        Object SpawnEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Tackle"), new Vector3(0.5F), 1.0F, 0, 2);
        MoveAnimation.AnimationFade(SpawnEntity, true, 1.0F, 0.0F, 2, 0);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
