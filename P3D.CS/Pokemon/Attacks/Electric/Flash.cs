using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class Flash : Attack
{
    public Flash()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 148;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 60;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Flash");
        Description = "The user flashes a bright light with enormous speed at the target. It cuts the user's accuracy though.";
        criticalChance = 1;
        isHMMove = true;
        target = Targets.OneAdjacentTarget;
        priority = 1;
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
        useEffectiveness = true;
        isHealingMove = false;
        removesSelfFrozen = false;
        isRecoilMove = false;

        immunityAffected = true;
        isDamagingMove = true;
        isProtectMove = false;

        hasSecondaryEffect = true;
        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        canHitSleeping = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.HighPriority;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Accuracy", 1, "", "move:flash");
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip, true);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Electric\Flash", 0.0F, 0);
        MoveAnimation.AnimationBackground(TextureManager.GetTexture(@"Textures\Battle\Electric\FlashBackground"), 0, 0, 1.5F, 0.9F, 0.2F, 0.025F, true, 1, 0, 6);
        MoveAnimation.AnimationColor(currentEntity, false, 0.2F, true, 0, 0, new Vector3(0.1f), 0.025F);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
