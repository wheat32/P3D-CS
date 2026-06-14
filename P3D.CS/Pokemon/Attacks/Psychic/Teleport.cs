using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class Teleport : Attack
{
    public Teleport()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 100;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Teleport");
        Description = "Use it to flee from any wild Pokémon. It can also warp to the last Pokémon Center visited.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.Self;
        priority = -6;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
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

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.IsTrainerBattle == true || battleScreen.IsRemoteBattle == true || battleScreen.IsPVPBattle == true)
        {
            // Fails due to trainer battle.
            battleScreen.BattleQuery.Add(new TextQueryObject("But " + Name + " failed!"));
        }
        else
        {
            Pokemon p = battleScreen.SelfPokemon;
            int trapped = battleScreen.FieldEffects.TrappedCounter.Self;
            if (own == false)
            {
                p = battleScreen.OpponentPokemon;
                trapped = battleScreen.FieldEffects.TrappedCounter.Opponent;
            }

            if (p.Ability.Name.ToLower() == "run away" || p.Item?.Name.ToLower() == "smoke ball" || trapped == 0 && BattleCalculation.CanRun(own, battleScreen) == true)
            {
                battleScreen.Battle.wildHasEscaped = true;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " fled from battle!"));
                battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("But " + Name + " failed, " + p.GetDisplayName() + " is trapped!"));
            }
        }
    }

}
