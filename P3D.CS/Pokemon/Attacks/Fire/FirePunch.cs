using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class FirePunch : Attack
{
    public FirePunch()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 7;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 75;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Fire Punch");
        Description = "The target is punched with a fiery fist. It may also leave the target with a burn.";
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
        removesSelfFrozen = false;
        hasSecondaryEffect = true;

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
        aiField2 = AIField.CanBurn;

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
            battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:firepunch");
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        int maxAmount = 12;
        int currentAmount = 0;
        while (currentAmount <= maxAmount)
        {
            Texture2D Texture = TextureManager.GetTexture(@"Textures\Battle\Fire\Ember", new Rectangle(0, 64, 32, 32), "");
            float xDest = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
            float yDest = 0.375F;
            float zDest = (float)((Core.Random.NextDouble() - 0.5) * 1.2);

            Vector3 Destination = new Vector3(xDest, yDest, zDest);

            Vector3 Position = new Vector3(0, -0.1f, 0);

            Vector3 Scale = new Vector3(0.375F);
            double startDelay = 1.5 * Core.Random.NextDouble();
            Entity FlameEntity = MoveAnimation.SpawnEntity(Position, Texture, Scale, 1.0F, (float)(startDelay));

            MoveAnimation.AnimationMove(FlameEntity, false, Destination.X, Destination.Y, Destination.Z, 0.02F, false, false, (float)(startDelay), 0.0F, 0.0075F);
            MoveAnimation.AnimationFade(FlameEntity, true, 0.4F, 0.0F, (float)(startDelay) + 1.5F, 0);
            System.Threading.Interlocked.Increment(ref currentAmount);
        }

        Entity FistEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Fire\FirePunch_Fist"), new Vector3(0.5F), 1.0F, 0, 2);
        MoveAnimation.AnimationOscillateMove(FistEntity, false, new Vector3(0, 0.02f, 0), 0.03f, true, 7, 0, 0.5f, 0, new Vector3(0, 1, 0));
        MoveAnimation.AnimationFade(FistEntity, true, 1.0F, 0.0F, 7, 0);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Fire\FirePunch", 0, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
