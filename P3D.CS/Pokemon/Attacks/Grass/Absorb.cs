using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class Absorb : Attack
{
    public Absorb()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 71;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 20;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Absorb");
        Description = "Inflicts damage on the target, then restores the user's HP based on the damage inflicted.";
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = true;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Absorbing;
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

        int damage = battleScreen.FieldEffects.LastDamage.Self;
        if (own == false)
        {
            damage = battleScreen.FieldEffects.LastDamage.Opponent;
        }
        int heal = (int)(Math.Ceiling((double)(damage / 2)));

        if (heal <= 0)
        {
            heal = 1;
        }

        if (op.Ability.Name.ToLower() == "liquid ooze" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
        {
            battleScreen.Battle.ReduceHP(heal, own, own, battleScreen, "Liquid Ooze damaged " + p.GetDisplayName() + "!", "liquidooze");
        }
        else
        {
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
                {
                    heal = (int)(Math.Ceiling((double)(damage * (80 / 100))));
                }
            }

            int healBlock = battleScreen.FieldEffects.HealBlock.Opponent;
            if (own == false)
            {
                healBlock = battleScreen.FieldEffects.HealBlock.Self;
            }
            if (healBlock == 0)
            {
                battleScreen.Battle.GainHP(heal, own, own, battleScreen, op.GetDisplayName() + " had its energy drained!", "move:absorb");
            }
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        int maxAmount = 12;
        int currentAmount = 0;
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Grass\Absorb", 0, 0);
        while (currentAmount <= maxAmount)
        {
            float yPos = (float)(Core.Random.Next(-1, 3) * 0.15);
            float zPos = (float)(Core.Random.Next(-3, 3) * 0.15);
            Object AbsorbEntity = MoveAnimation.SpawnEntity(new Vector3(0.0f, 0.0f, 0.0f), TextureManager.GetTexture(@"Textures\Battle\Grass\Absorb"), new Vector3(0.35F), 1, (float)(currentAmount * 0.8));
            MoveAnimation.AnimationMove(AbsorbEntity, true, -1.5, yPos, zPos, 0.03, false, true, (float)(currentAmount * 0.8), 0.0, 0.1, 0.5, 0.005F);

            System.Threading.Interlocked.Increment(currentAmount);
        }

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
