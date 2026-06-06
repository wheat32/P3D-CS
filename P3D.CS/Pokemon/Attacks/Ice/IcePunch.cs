using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class IcePunch : Attack
{
    public IcePunch()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 8;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 75;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Ice Punch");
        Description = "The target is punched with an icy fist. It may also leave the target frozen.";
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
        kingsrockAffected = false;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isPunchingMove = true;
        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanFreeze;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictFreeze(own == false, own, battleScreen, "", "move:icepunch");
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Ice\IcePunch_Crystals", 0, 0);
        int maxAmount = 10;
        int currentAmount = 0;
        while (currentAmount <= maxAmount)
        {
            Texture2D Texture = TextureManager.GetTexture(@"Textures\Battle\Ice\IcePunch_Crystals", new Rectangle(0, 0, 16, 16), "");
            float xPos = (float)(Core.Random.Next(-4, 4) / 8);
            float zPos = (float)(Core.Random.Next(-4, 4) / 8);

            Vector3 Position = new Vector3(xPos, -0.25f, zPos);
            Vector3 Destination = new Vector3(xPos - xPos * 2, 0, zPos - zPos * 2);
            Vector3 Scale = new Vector3(0.25F);
            double startDelay = 5.0 * Core.Random.NextDouble();
            Object IceEntity = MoveAnimation.SpawnEntity(Position, Texture, Scale, 1.0F, (float)(startDelay));
            MoveAnimation.AnimationMove(IceEntity, false, Destination.X, Destination.Y, Destination.Z, 0.0125F, false, true, (float)(startDelay), 0.0F);
            MoveAnimation.AnimationChangeTexture(IceEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ice\IcePunch_Crystals", new Rectangle(16, 0, 16, 16), ""), (float)(startDelay + 0.5), 0);
            MoveAnimation.AnimationChangeTexture(IceEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ice\IcePunch_Crystals", new Rectangle(0, 0, 16, 16), ""), (float)(startDelay + 1), 0);
            MoveAnimation.AnimationChangeTexture(IceEntity, false, TextureManager.GetTexture(@"Textures\Battle\Ice\IcePunch_Crystals", new Rectangle(16, 0, 16, 16), ""), (float)(startDelay + 1.5), 0);
            MoveAnimation.AnimationRotate(IceEntity, true, 0, 0, 0.125, 0, 0, 3, (float)(startDelay), 0, false);

            System.Threading.Interlocked.Increment(ref currentAmount);
        }
        Object FistEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Ice\IcePunch_Fist"), new Vector3(0.5F), 1, 5, 3);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Ice\IcePunch_Fist", 5, 0);
        MoveAnimation.AnimationFade(FistEntity, true, 1.0F, 0.0F, 8, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
