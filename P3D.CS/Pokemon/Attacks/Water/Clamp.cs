using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class Clamp : Attack
{
    public Clamp()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 128;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 35;
        Accuracy = 85;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Clamp");
        Description = "The target is clamped and squeezed by the user's very thick and sturdy shell for four to five turns.";
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
            if (battleScreen.FieldEffects.Clamp.Opponent == 0)
            {
                battleScreen.FieldEffects.Clamp.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " clamped " + op.GetDisplayName() + "!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Clamp.Self == 0)
            {
                battleScreen.FieldEffects.Clamp.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " clamped " + op.GetDisplayName() + "!"));
            }
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        float offsetLeft = -0.35;
        float offsetRight = 0.35;
        if (battleFlip == true)
        {
            offsetLeft = 0.35;
            offsetRight = -0.35;
        }
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Clamp", 0, 0);
        Object ClampEntityLeft = MoveAnimation.SpawnEntity(new Vector3(offsetLeft, -0.1f, offsetLeft), TextureManager.GetTexture(@"Textures\Battle\Water\Clamp_Left", new Rectangle(0, 0, 24, 64), ""), new Vector3(0.28F, 0.75F, 0.28F), 0.75F);
        Object ClampEntityRight = MoveAnimation.SpawnEntity(new Vector3(offsetRight, -0.1f, offsetRight), TextureManager.GetTexture(@"Textures\Battle\Water\Clamp_Right", new Rectangle(0, 0, 24, 64), ""), new Vector3(0.28F, 0.75F, 0.28F), 0.75F);
        MoveAnimation.AnimationMove(ClampEntityLeft, false, -0.1, -0.1, -0.1, 0.02, false, false, 0, 0);
        MoveAnimation.AnimationMove(ClampEntityRight, false, 0.1, -0.1, 0.1, 0.02, false, false, 0, 0);
        MoveAnimation.AnimationMove(ClampEntityLeft, true, -0.35, -0.1, -0.35, 0.02, false, false, 2, 0);
        MoveAnimation.AnimationMove(ClampEntityRight, true, 0.35, -0.1, 0.35, 0.02, false, false, 2, 0);
        Object SpawnEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Tackle"), new Vector3(0.5F), 1.0F, 2.5, 2);
        MoveAnimation.AnimationFade(SpawnEntity, true, 1.0F, 0.0F, 4.5F, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
