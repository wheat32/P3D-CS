using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class Gust : Attack
{
    public Gust()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 16;
        originalPP = 35;
        currentPP = 35;
        maxPP = 35;
        Power = 40;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Gust");
        Description = "A gust of wind is whipped up by wings and launched at the target to inflict damage.";
        criticalChance = 1;
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
        kingsrockAffected = true;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isWindMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        canHitInMidAir = true;
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int fly = 0;
        int bounce = 0;

        if (own == true)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
            bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
        }
        else
        {
            fly = battleScreen.FieldEffects.FlyCounter.Self;
            bounce = battleScreen.FieldEffects.BounceCounter.Self;
        }

        if (fly > 0 || bounce > 0)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Flying\Gust", 0, 0);
        Object GustEntity = MoveAnimation.SpawnEntity(new Vector3(0), TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), new Vector3(0.5F, 1.0F, 0.5F), 1.0F);
        MoveAnimation.AnimationMove(GustEntity, true, 2.0, 0.0, 0.0, 0.04, false, false, 0.0, 0.0);

        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 0.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 1, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 1.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 2.0, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 2.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 3.0, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 3.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 4.0, 0.5);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Flying\Gust", 0, 0);
        Object GustEntity = MoveAnimation.SpawnEntity(new Vector3(-2, 0, 0), TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), new Vector3(0.5F, 1.0F, 0.5F), 1.0F);
        MoveAnimation.AnimationMove(GustEntity, false, -0.05, 0.0, 0.0, 0.04, false, false, 0.0, 0.0);

        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 0.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 1, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 1.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 2.0, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 2.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 3.0, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 3.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 4.0, 0.5);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 4.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 5.0, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(32, 0, 32, 64), ""), 5.5, 0);
        MoveAnimation.AnimationChangeTexture(GustEntity, true, TextureManager.GetTexture(@"Textures\Battle\Flying\Gust", new Rectangle(0, 0, 32, 64), ""), 6.0, 0.5);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
