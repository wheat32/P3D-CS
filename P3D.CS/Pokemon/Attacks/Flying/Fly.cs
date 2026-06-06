using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class Fly : Attack
{
    public Fly()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 19;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 90;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Fly");
        Description = "The user soars, then strikes its target on the second turn. It can also be used for flying to any familiar town.";
        criticalChance = 1;
        isHMMove = true;
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
        kingsrockAffected = true;
        counterAffected = true;

        disabledWhileGravity = true;
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
        aiField2 = AIField.MultiTurn;
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int fly = battleScreen.FieldEffects.FlyCounter.Self;
        if (own == false)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
        }

        if (fly == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void PreAttack(bool own, BattleScreen battleScreen)
    {
        int fly = battleScreen.FieldEffects.FlyCounter.Self;
        if (own == false)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
        }

        if (fly == 0)
        {
            focusOpponentPokemon = false;
        }
        else
        {
            focusOpponentPokemon = true;
        }
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int fly = battleScreen.FieldEffects.FlyCounter.Self;
        if (own == false)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Fly!", "move:fly") == true)
                {
                    fly = 1;
                }
            }
        }

        if (fly == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " flew up high!"));
            if (own == true)
            {
                battleScreen.FieldEffects.FlyCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.FlyCounter.Opponent = 1;
            }
            return true;
        }
        else if (fly == 1)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.FlyCounter.Self = 2;
            }
            else
            {
                battleScreen.FieldEffects.FlyCounter.Opponent = 2;
            }
            return false;
        }
        return false;
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.FlyCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.FlyCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int fly = battleScreen.FieldEffects.FlyCounter.Self;
        if (own == false)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
        }

        if (fly == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void MoveFails(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.FlyCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.FlyCounter.Opponent = 0;
        }
        FailPokemonMoveAnimation(battleScreen, own);
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void AbsorbedBySubstitute(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void InflictedFlinch(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsSleeping(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsAttracted(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        int fly = battleScreen.FieldEffects.FlyCounter.Self;
        if (battleFlip == true)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
        }
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        if (fly == 0)
        {
            MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Flying\Fly_Start", 0, 0);
            MoveAnimation.AnimationFade(null, false, 0.2F, 0.0F, 0, 0);
            Object FlyEntity = MoveAnimation.SpawnEntity(new Vector3(0), TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 0.0F);
            MoveAnimation.AnimationFade(FlyEntity, false, 0.2F, 1.0F, 0, 0);
            MoveAnimation.AnimationMove(FlyEntity, true, 0.0, 2.0, 0.0, 0.06, false, false, 1.4F, 0.0, 0.06, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 32, 32, 32), ""), 1.3F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 64, 32, 32), ""), 1.4F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 96, 32, 32), ""), 1.5F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 128, 32, 32), ""), 1.6F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 160, 32, 32), ""), 1.7F, 0);

            battleScreen.BattleQuery.Add(MoveAnimation);
        }
        else
        {
            MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Flying\Fly_Start", 0, 0);
            Object FlyEntity = MoveAnimation.SpawnEntity(new Vector3(0, 0.9f, 0), TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1.0F);
            MoveAnimation.AnimationMove(FlyEntity, true, 2.0, 0.5, 0, 0.07, false, false, 0.0F, 0.0, 0.035, 0);

            battleScreen.BattleQuery.Add(MoveAnimation);
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        int fly = battleScreen.FieldEffects.FlyCounter.Self;
        if (battleFlip == true)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
        }

        if (fly == 2)
        {
            AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
            float ModelOffset = 0.0F;
            if (currentEntity.ModelPath != "")
            {
                ModelOffset += 0.5F;
            }
            MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Flying\Fly_Hit", 0, 0);
            Object FlyEntity = MoveAnimation.SpawnEntity(new Vector3(-2, 0.9f, 0), TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1.0F);
            MoveAnimation.AnimationMove(FlyEntity, true, 0.0, 0.0F + ModelOffset, 0.0, 0.07, false, false, 0.0, 0.0, 0.035, 3);

            if (battleFlip == false)
            {
                MoveAnimation.AnimationFade(battleScreen.SelfPokemonNPC, false, 1, 1.0F, 0, 0);
            }
            else
            {
                MoveAnimation.AnimationFade(battleScreen.OpponentPokemonNPC, false, 1, 1.0F, 0, 0);
            }

            battleScreen.BattleQuery.Add(MoveAnimation);
        }
    }

    public override void InternalFailPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        float FadeDelay = 0.0F;
        float FadeSpeed = 1.0F;

        int fly = battleScreen.FieldEffects.FlyCounter.Self;
        if (battleFlip == true)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
        }

        if (battleScreen.FieldEffects.Gravity > 0)
        {
            FadeDelay = 2.3F;
            FadeSpeed = 0.2F;
            Object FlyEntity = MoveAnimation.SpawnEntity(new Vector3(0, 2, 0), TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 160, 32, 32), ""), new Vector3(0.5F), 1.0F);
            MoveAnimation.AnimationMove(FlyEntity, false, 0.0, 0.0, 0.0, 0.1F, false, false, 0.0F, 0.0, 0.1F, 1);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 128, 32, 32), ""), 0.0F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 96, 32, 32), ""), 0.1F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 64, 32, 32), ""), 0.2F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 32, 32, 32), ""), 0.3F, 0);
            MoveAnimation.AnimationChangeTexture(FlyEntity, false, TextureManager.GetTexture(@"Textures\Battle\Flying\Fly", new Rectangle(0, 0, 32, 32), ""), 0.4F, 0);
            MoveAnimation.AnimationFade(FlyEntity, true, FadeSpeed, 0.0F, FadeDelay + 0.1F, 0, 1);
        }
        if (battleFlip == false)
        {
            MoveAnimation.AnimationFade(battleScreen.SelfPokemonNPC, false, FadeSpeed, 1.0F, FadeDelay, 0);
        }
        else
        {
            MoveAnimation.AnimationFade(battleScreen.OpponentPokemonNPC, false, FadeSpeed, 1.0F, FadeDelay, 0);
        }
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
