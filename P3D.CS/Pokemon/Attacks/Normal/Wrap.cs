using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Wrap : Attack
{
    public Wrap()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 35;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 15;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Wrap");
        Description = "A long body or vines are used to wrap and squeeze the target for four to five turns.";
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
        kingsrockAffected = true;
        counterAffected = true;

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
            if (battleScreen.FieldEffects.Wrap.Opponent == 0)
            {
                battleScreen.FieldEffects.Wrap.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was wrapped by " + p.GetDisplayName() + "!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Wrap.Self == 0)
            {
                battleScreen.FieldEffects.Wrap.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was wrapped by " + p.GetDisplayName() + "!"));
            }
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Wrap", 5.0F, 0);
        Object WrapEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 0, 80, 40), ""), new Vector3(1.0F, 0.5F, 1.0F), 1, 0, 0.75);
        MoveAnimation.AnimationChangeTexture(WrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 40, 80, 40), ""), 0.75, 0.75);
        MoveAnimation.AnimationChangeTexture(WrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 80, 80, 40), ""), 1.5, 0.75);
        MoveAnimation.AnimationChangeTexture(WrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 120, 80, 40), ""), 2.25, 0.75);
        MoveAnimation.AnimationChangeTexture(WrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 160, 80, 40), ""), 3, 0.75);
        MoveAnimation.AnimationChangeTexture(WrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 200, 80, 40), ""), 3.75, 0.75);
        MoveAnimation.AnimationScale(null, false, 0.75F, 1.0F, 0.75F, 0.02F, 5, 0);
        MoveAnimation.AnimationScale(WrapEntity, false, 0.75F, 0.5F, 0.75F, 0.02F, 5, 0);
        MoveAnimation.AnimationScale(null, false, 1.0F, 1.0F, 1.0F, 0.04F, 7, 0);
        MoveAnimation.AnimationScale(WrapEntity, false, 1.0F, 0.5F, 1.0F, 0.04F, 7, 0);
        MoveAnimation.AnimationScale(null, false, 0.75F, 1.0F, 0.75F, 0.02F, 9, 0);
        MoveAnimation.AnimationScale(WrapEntity, false, 0.75F, 0.5F, 0.75F, 0.02F, 9, 0);
        MoveAnimation.AnimationScale(null, false, 1.0F, 1.0F, 1.0F, 0.04F, 11, 0);
        MoveAnimation.AnimationScale(WrapEntity, false, 1.0F, 0.5F, 1.0F, 0.04F, 11, 0);
        MoveAnimation.AnimationFade(WrapEntity, true, 0.03, 0.0, 11, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
