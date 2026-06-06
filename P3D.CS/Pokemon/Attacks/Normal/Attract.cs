using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Attract : Attack
{
    public Attract()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 213;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Attract");
        Description = "If it is the opposite gender of the user, the target becomes infatuated and less likely to attack.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Infatuation;
        aiField2 = AIField.Nothing;
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

        if (p.Gender != Pokemon.Genders.Genderless && op.Gender != Pokemon.Genders.Genderless && op.Gender != p.Gender)
        {
            if (op.Ability.Name.ToLower() != "aroma veil")
            {
                if (battleScreen.Battle.InflictInfatuate(own == false, own, battleScreen, "", "move:attract") == false)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                }
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Aroma Veil protected " + op.GetDisplayName() + " from " + Name + "!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        for (int i = 0; i <= 6; i++)
        {
            Object HeartEntity = MoveAnimation.SpawnEntity(null, TextureManager.GetTexture(@"Textures\Battle\Normal\Attract"), new Vector3(0.25F), 1.0F, (float)(i * 0.2));

            MoveAnimation.AnimationMove(HeartEntity, true, 2.0, 0.0, 0.0, 0.075, false, false, (float)(i * 0.2), 0.0);
            i += 1;
        }
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Attract", 0, 0);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        for (int i = 0; i <= 6; i++)
        {
            Object HeartEntity = MoveAnimation.SpawnEntity(new Vector3(-2.0f, 0.0f, 0.0f), TextureManager.GetTexture(@"Textures\Battle\Normal\Attract"), new Vector3(0.25F), 1.0F, (float)(i * 0.2));

            MoveAnimation.AnimationMove(HeartEntity, false, 0.0, 0.0, 0.0, 0.06, false, false, (float)(i * 0.2), 0.0);
            float zPos = (float)(Core.Random.Next(-2, 2) * 0.2);
            MoveAnimation.AnimationMove(HeartEntity, false, 0.0, 0.25, zPos, 0.01, false, false, (float)(1 + i * 0.2), 0.0);
            MoveAnimation.AnimationFade(HeartEntity, true, 0.02, 0.0, (float)(2 + i * 0.2), 0.0);
            i += 1;
        }

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
