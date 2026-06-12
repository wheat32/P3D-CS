using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Poison;

public class PoisonSting : Attack
{
    public PoisonSting()
    {
        // #Definitions
        type = new Element(Element.Types.Poison);
        ID = 40;
        originalPP = 35;
        currentPP = 35;
        maxPP = 35;
        Power = 15;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Poison Sting");
        Description = "The user stabs the target with a poisonous stinger. This may also poison the target.";
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
        counterAffected = true;

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
        aiField2 = AIField.CanPoison;

        effectChances.Add(30);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);

        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictPoison(own == false, own, battleScreen, false, "", "move:poisonsting");
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        int TextureYOffset = 0;
        if (battleFlip == true)
        {
            TextureYOffset = 16;
        }
        Entity StingerEntity = MoveAnimation.SpawnEntity(Vector3.Zero, TextureManager.GetTexture(@"Textures\Battle\Poison\Stinger", new Rectangle(0, TextureYOffset, 16, 16), ""), new Vector3(0.2F), 1.0F);

        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Poison\PoisonSting_Start", 0, 0);
        MoveAnimation.AnimationMove(StingerEntity, true, 2.0f, 0.0f, 0.0f, 0.08f, false, false, 0.0f, 0.0f);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        int TextureYOffset = 0;
        if (battleFlip == true)
        {
            TextureYOffset = 16;
        }

        Entity StingerEntity = MoveAnimation.SpawnEntity(new Vector3(-2.0f, 0, 0.0f), TextureManager.GetTexture(@"Textures\Battle\Poison\Stinger", new Rectangle(0, TextureYOffset, 16, 16), ""), new Vector3(0.2F), 1);

        MoveAnimation.AnimationMove(StingerEntity, true, 0.0f, 0.0f, 0.0f, 0.08f, false, false, 0.0f, 0.0f);

        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Poison\PoisonSting_Hit", 1, 0);

        Vector3 Bubble1Position = new Vector3(-0.25f, -0.25f, -0.25f);
        Vector3 Bubble2Position = new Vector3(0, -0.25f, 0);
        Vector3 Bubble3Position = new Vector3(0.25f, -0.25f, 0.25f);

        Entity BubbleEntity1 = MoveAnimation.SpawnEntity(Bubble1Position, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1, 2, 1);

        MoveAnimation.AnimationChangeTexture(BubbleEntity1, false, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 32, 32, 32), ""), 3, 1);

        Entity BubbleEntity2 = MoveAnimation.SpawnEntity(Bubble2Position, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1, 3, 1);

        MoveAnimation.AnimationChangeTexture(BubbleEntity1, true, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 64, 32, 32), ""), 4, 1);
        MoveAnimation.AnimationChangeTexture(BubbleEntity2, false, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 32, 32, 32), ""), 4, 1);

        Entity BubbleEntity3 = MoveAnimation.SpawnEntity(Bubble3Position, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1, 4, 1);

        MoveAnimation.AnimationChangeTexture(BubbleEntity2, true, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 64, 32, 32), ""), 5, 1);
        MoveAnimation.AnimationChangeTexture(BubbleEntity3, false, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 32, 32, 32), ""), 5, 1);

        MoveAnimation.AnimationChangeTexture(BubbleEntity3, true, TextureManager.GetTexture(@"Textures\Battle\Poison\Bubble", new Rectangle(0, 64, 32, 32), ""), 6, 1);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
