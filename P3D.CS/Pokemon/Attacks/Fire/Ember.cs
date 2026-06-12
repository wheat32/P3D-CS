using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class Ember : Attack
{
    public Ember()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 52;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 40;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Ember");
        Description = "The target is attacked with small flames. It may also leave the target with a burn.";
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
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
        aiField1 = AIField.Damage;
        aiField2 = AIField.CanBurn;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:ember");
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        int TextureYOffset = 0;
        if (battleFlip == true)
        {
            TextureYOffset = 32;
        }
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Fire\Ember_Start", 0, 0);
        Entity FireballEntity = MoveAnimation.SpawnEntity(new Vector3(0), TextureManager.GetTexture(@"Textures\Battle\Fire\FireBall", new Rectangle(0, TextureYOffset, 32, 32), ""), new Vector3(0.5F), 1.0F);
        MoveAnimation.AnimationMove(FireballEntity, true, 2.0f, 0.0f, 0.0f, 0.05f, false, true, 0.0f, 0.0f, -0.5f);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        int TextureYOffset = 0;
        if (battleFlip == true)
        {
            TextureYOffset = 32;
        }
        Entity FireballEntity = MoveAnimation.SpawnEntity(new Vector3(-2.0f, 0.0f, 0.0f), TextureManager.GetTexture(@"Textures\Battle\Fire\FireBall", new Rectangle(0, TextureYOffset, 32, 32), ""), new Vector3(0.5F), 1.0F);
        MoveAnimation.AnimationMove(FireballEntity, true, -0.05f, 0.0f, 0.0f, 0.05f, false, true, 0.0f, 1.0f, -0.5f);

        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Fire\Ember_Hit", 4, 0);

        Entity FireEntity1 = MoveAnimation.SpawnEntity(new Vector3(-0.25f, -0.25f, -0.25f), TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1, 3, 0);
        Entity FireEntity2 = MoveAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1, 3, 0);
        Entity FireEntity3 = MoveAnimation.SpawnEntity(new Vector3(0.25f, -0.25f, 0.25f), TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1, 3, 0);

        MoveAnimation.AnimationChangeTexture(FireEntity1, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 32, 32, 32), ""), 3.75f, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity2, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 32, 32, 32), ""), 3.75f, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity3, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 32, 32, 32), ""), 3.75f, 0);

        MoveAnimation.AnimationChangeTexture(FireEntity1, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 64, 32, 32), ""), 4.5f, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity2, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 64, 32, 32), ""), 4.5f, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity3, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 64, 32, 32), ""), 4.5f, 0);

        MoveAnimation.AnimationChangeTexture(FireEntity1, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 96, 32, 32), ""), 5.25f, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity2, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 96, 32, 32), ""), 5.25f, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity3, false, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 96, 32, 32), ""), 5.25f, 0);

        MoveAnimation.AnimationChangeTexture(FireEntity1, true, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 128, 32, 32), ""), 6, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity2, true, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 128, 32, 32), ""), 6, 0);
        MoveAnimation.AnimationChangeTexture(FireEntity3, true, TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 128, 32, 32), ""), 6, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
