using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class WaterGun : Attack
{
    public WaterGun()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 55;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 40;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Water Gun");
        Description = "The target is blasted with a forceful shot of water.";
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
        Object WaterEntity = MoveAnimation.SpawnEntity(new Vector3(0), TextureManager.GetTexture(@"Textures\Battle\Water\WaterGun", new Rectangle(0, 0, 16, 16), ""), new Vector3(0.5F), 0.75F);
        MoveAnimation.AnimationMove(WaterEntity, true, 2, 0.5, 0, 0.075, false, false, 0, 0, 0.05);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Watergun_Start", 0, 0);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        Object WaterEntity = MoveAnimation.SpawnEntity(new Vector3(-2, 1, 0), TextureManager.GetTexture(@"Textures\Battle\Water\WaterGun", new Rectangle(0, 0, 16, 16), ""), new Vector3(0.5F), 0.5F);
        MoveAnimation.AnimationMove(WaterEntity, true, 0, 0, 0, 0.075, false, false, 0, 0, 0.035);

        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Watergun_Hit", 3, 0);
        Object HitEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Water\WaterGun", new Rectangle(0, 16, 16, 16), ""), new Vector3(0.5F), 0.5F, 3, 1);
        MoveAnimation.AnimationFade(HitEntity, true, 1.0F, 0.0F, 5, 0);

        Vector3 WaterDrop1Position = new Vector3(-0.25f, 0.25f, -0.25f);
        Vector3 WaterDrop2Position = new Vector3(0, 0.25f, 0);
        Vector3 WaterDrop3Position = new Vector3(0.25f, 0.25f, 0.25f);

        Entity WaterDropEntity1 = MoveAnimation.SpawnEntity(WaterDrop1Position, TextureManager.GetTexture(@"Textures\Battle\Water\WaterGun", new Rectangle(0, 32, 16, 16), ""), new Vector3(0.5F), 0.75F, 5, 0);
        Entity WaterDropEntity2 = MoveAnimation.SpawnEntity(WaterDrop2Position, TextureManager.GetTexture(@"Textures\Battle\Water\WaterGun", new Rectangle(0, 32, 16, 16), ""), new Vector3(0.5F), 0.75F, 5, 0);
        Entity WaterDropEntity3 = MoveAnimation.SpawnEntity(WaterDrop3Position, TextureManager.GetTexture(@"Textures\Battle\Water\WaterGun", new Rectangle(0, 32, 16, 16), ""), new Vector3(0.5F), 0.75F, 5, 0);

        MoveAnimation.AnimationMove(WaterDropEntity1, true, WaterDrop1Position.X, -0.25, WaterDrop1Position.Z, 0.05F, false, false, 5, 0);
        MoveAnimation.AnimationMove(WaterDropEntity2, true, WaterDrop2Position.X, -0.25, WaterDrop2Position.Z, 0.05F, false, false, 5, 0);
        MoveAnimation.AnimationMove(WaterDropEntity3, true, WaterDrop3Position.X, -0.25, WaterDrop3Position.Z, 0.05F, false, false, 5, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
