using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class TailWhip : Attack
{
    public TailWhip()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 39;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Tail Whip");
        Description = "The user wags its tail cutely, making opposing Pokémon less wary and lowering their Defense stat.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
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
        immunityAffected = false;
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

        aiField1 = AIField.LowerDefense;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Defense", 1, "", "move:tailwhip");
        if (b == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationTurnNPC(2, 0, 0, 1, 0.6F, 1);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\TailWhip", 1, 0);
        MoveAnimation.AnimationOscillateMove(null, false, new Vector3(0, 0, -0.075f), 0.035, true, 3, 1, 0, 0, new Vector3(0, 0, 1));
        MoveAnimation.AnimationTurnNPC(2, 5, 0.5, 3, 0.4F, -1);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
