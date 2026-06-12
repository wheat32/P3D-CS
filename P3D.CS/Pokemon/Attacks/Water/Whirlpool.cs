using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class Whirlpool : Attack
{
    public Whirlpool()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 250;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 35;
        Accuracy = 85;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Whirlpool");
        Description = "Traps foes in a violent swirling whirlpool for four to five turns.";
        criticalChance = 1;
        isHMMove = true;
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

        aiField1 = AIField.Damage;
        aiField2 = AIField.Trap;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int dive = battleScreen.FieldEffects.DiveCounter.Opponent;
        if (own == false)
        {
            dive = battleScreen.FieldEffects.DiveCounter.Self;
        }

        if (dive > 0)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
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

        int turns = 4;
        if (Core.Random.Next(0, 100) < 50)
        {
            turns = 5;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "grip claw" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                turns = 5;
            }
        }

        if (own == true)
        {
            if (battleScreen.FieldEffects.Whirlpool.Opponent == 0)
            {
                battleScreen.FieldEffects.Whirlpool.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was trapped in the vortex!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Whirlpool.Self == 0)
            {
                battleScreen.FieldEffects.Whirlpool.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was trapped in the vortex!"));
            }
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip, true);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Whirlpool", 0.0F, 0);
        Entity WhirlpoolEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.3F, 0), TextureManager.GetTexture(@"Textures\Battle\Water\Whirlpool"), new Vector3(0.0F), 1.0F, 0.0F, 0.0F);
        MoveAnimation.AnimationRotate(WhirlpoolEntity, false, (float)(MathHelper.Pi * 1.5f), 0, 0, (float)(MathHelper.Pi * 1.5f), 0, 0, 0, 0, false);
        MoveAnimation.AnimationRotate(WhirlpoolEntity, false, 0, 0, 0.2F, 0, 0, 10.0F, 0.0F, 0.0F, true);
        MoveAnimation.AnimationScale(WhirlpoolEntity, false, 1.0F, 1.0F, 1.0F, 0.025F, 0.0F, 0.0F);
        MoveAnimation.AnimationScale(WhirlpoolEntity, true, 0.0F, 0.0F, 0.0F, 0.025F, 5.0F, 0.0F);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
