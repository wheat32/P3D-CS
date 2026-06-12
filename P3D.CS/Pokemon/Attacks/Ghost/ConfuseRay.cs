using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class ConfuseRay : Attack
{
    public ConfuseRay()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 109;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Confuse Ray");
        Description = "The target is exposed to a sinister ray that triggers confusion.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Confusion;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.Battle.InflictConfusion(own == false, own, battleScreen, "", "move:confuseray") == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(null, battleFlip);

        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Ghost\ConfuseRay_Start", 0.0F, 0);

        Entity RayEntity = MoveAnimation.SpawnEntity(currentEntity.Position, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), new Vector3(0.5F), 0.0F);
        MoveAnimation.AnimationFade(RayEntity, false, 0.025F, 1.0F, 0, 0);
        MoveAnimation.AnimationMove(RayEntity, false, 1.5f, 0, 0, 0.025f, false, false, 0, 0, 0.0125f);
        MoveAnimation.AnimationOscillateMove(RayEntity, true, new Vector3(0, 0.075f, 0), 0.02f, true, 6, 0, 0, 1);

        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 0.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 1.0f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 1.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 2.0f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 2.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 3, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 3.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 4, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(null, battleFlip);
        Vector3 SpawnPosition = currentEntity.Position;
        if (battleFlip == false)
        {
            SpawnPosition.X = currentEntity.Position.X - 1.5F;
        }
        else
        {
            SpawnPosition.X = currentEntity.Position.X + 1.5F;
        }

        if (currentEntity.Model != null)
        {
            SpawnPosition.Y = currentEntity.Position.Y - 0.5F;
        }
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Ghost\ConfuseRay_End", 0.0F, 0);

        Entity RayEntity = MoveAnimation.SpawnEntity(SpawnPosition, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), new Vector3(0.5F), 1.0F);
        MoveAnimation.AnimationMove(RayEntity, false, 1.5f, 0, 0, 0.025f, false, false, 0, 0, 0.0125f);
        MoveAnimation.AnimationOscillateMove(RayEntity, false, new Vector3(0, 0.075f, 0), 0.02f, true, 6, 0, 0, 1);
        MoveAnimation.AnimationOscillateMove(RayEntity, false, new Vector3(0, 0, 0.2f), 0.075f, true, 7.5f, 4, 0, 1);

        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 0.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 1.0f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 1.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 2.0f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 2.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 3, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 3.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 4, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 4.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 5.0f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 5.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 6.0f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(16, 0, 16, 16), ""), 6.5f, 0);
        MoveAnimation.AnimationChangeTexture(RayEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ghost\ConfuseRay", new Rectangle(0, 0, 16, 16), ""), 7, 0);

        MoveAnimation.AnimationFade(RayEntity, true, 0.035f, 0, 6.75f, 0);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
