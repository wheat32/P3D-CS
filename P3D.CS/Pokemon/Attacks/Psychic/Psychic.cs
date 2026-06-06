using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class Psychic : Attack
{
    public Psychic()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 94;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 90;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Psychic");
        Description = "The target is hit by a strong telekinetic force. It may also reduce the target's Sp. Def. stat.";
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
        removesSelfFrozen = false;
        hasSecondaryEffect = true;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanLowerSpDefense;

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
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Defense", 1, "", "move:psychic");
        }
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip, true);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Psychic\Psychic", 0.0F, 0);
        MoveAnimation.AnimationBackground(TextureManager.GetTexture(@"Textures\Battle\Psychic\PsychicBackground"), 0, 0, 9.0F, 0.6F, 0.075F, 0.075F, true, 11, 2, 6);

        MoveAnimation.AnimationOscillateMove(null, false, new Vector3(0, 0, 0.05f), 0.035, true, 4.5, 1, 0.5, 0, new Vector3(0, 0, 1));

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
