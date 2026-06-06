using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Pound : Attack
{
    public Pound()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 1;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 40;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Pound");
        Description = "The target is physically pounded with a long tail or a foreleg, etc.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        // Generic Secondary effects
        hasSecondaryEffect = false;
        isHealingMove = false;
        isDamagingMove = true;
        isProtectMove = false;
        isOneHitKOMove = false;
        isRecoilMove = false;
        isTrappingMove = false;
        removesSelfFrozen = false;

        // Interacts with other moves/effects
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = true;
        isAffectedBySubstitute = true;
        immunityAffected = true;
        isWonderGuardAffected = true;
        disabledWhileGravity = false;

        // ignore stats status or positioning?
        useAccEvasion = true;
        canHitInMidAir = false;
        canHitUnderground = false;
        canHitUnderwater = false;
        canHitSleeping = true;
        canGainSTAB = true;
        useOpponentDefense = true;
        useOpponentEvasion = true;
        useEffectiveness = true;

        // categories
        makesContact = true;
        isPulseMove = false;
        isBulletMove = false;
        isJawMove = false;
        isDanceMove = false;
        isExplosiveMove = false;
        isPowderMove = false;
        isPunchingMove = false;
        isSlicingMove = false;
        isSoundMove = false;
        isWindMove = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;
        aiField3 = AIField.Nothing;
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Pound", 0.5, 2.5);
        Object PoundEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Pound"), new Vector3(0.5F), 1, 0, 3);
        MoveAnimation.AnimationFade(PoundEntity, true, 1.0F, 0.0F, 3, 0);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
