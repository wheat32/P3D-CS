using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Poison;

public class ToxicSpikes : Attack
{
    public ToxicSpikes()
    {
        // #Definitions
        type = new Element(Element.Types.Poison);
        ID = 390;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Toxic Spikes");
        Description = "The user lays a trap of poison spikes at the opponent's feet. They poison opponents that switch into battle.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
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


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int spikes = 0;
        if (own == true)
        {
            spikes = battleScreen.FieldEffects.ToxicSpikes.Self;
        }
        else
        {
            spikes = battleScreen.FieldEffects.ToxicSpikes.Opponent;
        }
        if (spikes < 2)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.ToxicSpikes.Self += 1;
                battleScreen.BattleQuery.Add(new TextQueryObject("Poison spikes were scattered all around the feet of the foe's team!"));
            }
            else
            {
                battleScreen.FieldEffects.ToxicSpikes.Opponent += 1;
                battleScreen.BattleQuery.Add(new TextQueryObject("Poison spikes were scattered all around the feet of your team!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
