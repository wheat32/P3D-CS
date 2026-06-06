using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class Mist : Attack
{
    public Mist()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 54;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Mist");
        Description = "The user cloaks its body with a white mist that prevents any of its stats from being cut for five turns.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllOwn;
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
        bool b = true;

        if (own == true)
        {
            if (battleScreen.FieldEffects.Mist.Self == 0)
            {
                battleScreen.FieldEffects.Mist.Self = 5;
                battleScreen.BattleQuery.Add(new TextQueryObject("Your team became shrouded in mist!"));
            }
            else
            {
                b = false;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Mist.Opponent == 0)
            {
                battleScreen.FieldEffects.Mist.Opponent = 5;
                battleScreen.BattleQuery.Add(new TextQueryObject("The opponent team became shrouded in mist!"));
            }
            else
            {
                b = false;
            }
        }


        if (b == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
