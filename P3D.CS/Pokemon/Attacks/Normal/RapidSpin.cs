using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class RapidSpin : Attack
{
    public RapidSpin()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 229;
        originalPP = 40;
        currentPP = 40;
        maxPP = 40;
        Power = 50;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Rapid Spin");
        Description = "A spin attack that can also eliminate such moves as Bind, Wrap, Leech Seed, and Spikes. This also raises the user's Speed stat.";
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
        aiField2 = AIField.RaiseSpeed;
        aiField3 = AIField.Support;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Speed", 1, "", "move:rapidspin");

            if (own == true)
            {
                battleScreen.FieldEffects.Bind.Self = 0;
                battleScreen.FieldEffects.Clamp.Self = 0;
                battleScreen.FieldEffects.FireSpin.Self = 0;
                battleScreen.FieldEffects.LeechSeed.Self = 0;
                battleScreen.FieldEffects.MagmaStorm.Self = 0;
                battleScreen.FieldEffects.SandTomb.Self = 0;
                battleScreen.FieldEffects.Spikes.Opponent = 0;
                battleScreen.FieldEffects.StealthRock.Opponent = 0;
                battleScreen.FieldEffects.ToxicSpikes.Opponent = 0;
                battleScreen.FieldEffects.Whirlpool.Self = 0;
                battleScreen.FieldEffects.Wrap.Self = 0;
                battleScreen.FieldEffects.Infestation.Self = 0;
                battleScreen.FieldEffects.StickyWeb.Opponent = 0;
            }
            else
            {
                battleScreen.FieldEffects.Bind.Opponent = 0;
                battleScreen.FieldEffects.Clamp.Opponent = 0;
                battleScreen.FieldEffects.FireSpin.Opponent = 0;
                battleScreen.FieldEffects.LeechSeed.Opponent = 0;
                battleScreen.FieldEffects.MagmaStorm.Opponent = 0;
                battleScreen.FieldEffects.SandTomb.Opponent = 0;
                battleScreen.FieldEffects.Spikes.Self = 0;
                battleScreen.FieldEffects.StealthRock.Self = 0;
                battleScreen.FieldEffects.ToxicSpikes.Self = 0;
                battleScreen.FieldEffects.Whirlpool.Opponent = 0;
                battleScreen.FieldEffects.Wrap.Opponent = 0;
                battleScreen.FieldEffects.Infestation.Opponent = 0;
                battleScreen.FieldEffects.StickyWeb.Self = 0;
            }

    }

}
