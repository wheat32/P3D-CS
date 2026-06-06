using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class HyperspaceHole : Attack
{
    public HyperspaceHole()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 593;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 50;
        Accuracy = 0;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Hyperspace Hole");
        Description = "Using a hyperspace hole, the user appears right next to the target and strikes. This also hits a target using a move such as Protect or Detect.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
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
        useAccEvasion = false;
        canHitUnderwater = false;
        canHitUnderground = false;
        canHitInMidAir = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CannotMiss;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;

        if (own == true)
        {
            if (battleScreen.FieldEffects.DetectCounter.Opponent > 0 || battleScreen.FieldEffects.ProtectCounter.Opponent > 0 || battleScreen.FieldEffects.KingsShieldCounter.Opponent > 0 || battleScreen.FieldEffects.SpikyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.BanefulBunkerCounter.Opponent > 0 || battleScreen.FieldEffects.CraftyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.MatBlockCounter.Opponent > 0 || battleScreen.FieldEffects.WideGuardCounter.Opponent > 0 || battleScreen.FieldEffects.QuickGuardCounter.Opponent > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Hyperspace Hole lifted " + op.GetDisplayName() + "'s protection!"));
            }
            battleScreen.FieldEffects.DetectCounter.Opponent = 0;
            battleScreen.FieldEffects.ProtectCounter.Opponent = 0;
            battleScreen.FieldEffects.KingsShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.SpikyShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.BanefulBunkerCounter.Opponent = 0;
            battleScreen.FieldEffects.CraftyShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.MatBlockCounter.Opponent = 0;
            battleScreen.FieldEffects.WideGuardCounter.Opponent = 0;
            battleScreen.FieldEffects.QuickGuardCounter.Opponent = 0;
        }
        else
        {
            op = battleScreen.SelfPokemon;
            if (battleScreen.FieldEffects.DetectCounter.Self > 0 || battleScreen.FieldEffects.ProtectCounter.Self > 0 || battleScreen.FieldEffects.KingsShieldCounter.Self > 0 || battleScreen.FieldEffects.SpikyShieldCounter.Self > 0 || battleScreen.FieldEffects.BanefulBunkerCounter.Self > 0 || battleScreen.FieldEffects.CraftyShieldCounter.Self > 0 || battleScreen.FieldEffects.MatBlockCounter.Self > 0 || battleScreen.FieldEffects.WideGuardCounter.Self > 0 || battleScreen.FieldEffects.QuickGuardCounter.Self > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Hyperspace Hole lifted " + op.GetDisplayName() + "'s protection!"));
            }
            battleScreen.FieldEffects.DetectCounter.Self = 0;
            battleScreen.FieldEffects.ProtectCounter.Self = 0;
            battleScreen.FieldEffects.KingsShieldCounter.Self = 0;
            battleScreen.FieldEffects.SpikyShieldCounter.Self = 0;
            battleScreen.FieldEffects.BanefulBunkerCounter.Self = 0;
            battleScreen.FieldEffects.CraftyShieldCounter.Self = 0;
            battleScreen.FieldEffects.MatBlockCounter.Self = 0;
            battleScreen.FieldEffects.WideGuardCounter.Self = 0;
            battleScreen.FieldEffects.QuickGuardCounter.Self = 0;
        }

    }

}
