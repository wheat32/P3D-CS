using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class Tailwind : Attack
{
    public Tailwind()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 366;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Tailwind");
        Description = "The user whips up a turbulent whirlwind that ups the Speed stat of the user and its allies for four turns.";
        criticalChance = 1;
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
        mirrorMoveAffected = false;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;
        isWindMove = true;
        isDamagingMove = false;
        isProtectMove = false;

        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.RaiseSpeed;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.TailWind.Self == 0)
            {
                battleScreen.FieldEffects.TailWind.Self = 5;
                battleScreen.BattleQuery.Add(new TextQueryObject("The Tailwind blew from behind the team!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Tailwind failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.TailWind.Opponent == 0)
            {
                battleScreen.FieldEffects.TailWind.Opponent = 5;
                battleScreen.BattleQuery.Add(new TextQueryObject("The Tailwind blew from behind the team!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Tailwind failed!"));
            }
        }
    }

}
