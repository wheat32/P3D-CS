using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Feint : Attack
{
    public Feint()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 364;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 30;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Feint");
        Description = "An attack that hits a target using Protect or Detect. This also lifts the effects of those moves.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 2;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isPunchingMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;

        if (own == true)
        {
            if (battleScreen.FieldEffects.DetectCounter.Opponent > 0 || battleScreen.FieldEffects.ProtectCounter.Opponent > 0 || battleScreen.FieldEffects.KingsShieldCounter.Opponent > 0 || battleScreen.FieldEffects.SpikyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.BanefulBunkerCounter.Opponent > 0 || battleScreen.FieldEffects.CraftyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.MatBlockCounter.Opponent > 0 || battleScreen.FieldEffects.WideGuardCounter.Opponent > 0 || battleScreen.FieldEffects.QuickGuardCounter.Opponent > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Feint lifted " + op.GetDisplayName() + "'s protection!"));
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
                battleScreen.BattleQuery.Add(new TextQueryObject("Feint lifted " + op.GetDisplayName() + "'s protection!"));
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
