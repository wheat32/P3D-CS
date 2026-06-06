using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class FocusEnergy : Attack
{
    public FocusEnergy()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 116;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Focus Energy");
        Description = "The user takes a deep breath and focuses so that critical hits land more easily.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = true;
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
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (own == true)
        {
            if (battleScreen.FieldEffects.FocusEnergy.Self == 0)
            {
                battleScreen.FieldEffects.FocusEnergy.Self = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " got pumped!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.FocusEnergy.Opponent == 0)
            {
                battleScreen.FieldEffects.FocusEnergy.Opponent = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " got pumped!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
