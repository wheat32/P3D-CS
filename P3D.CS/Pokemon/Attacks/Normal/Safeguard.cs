using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Safeguard : Attack
{
    public Safeguard()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 219;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Safeguard");
        Description = "The user creates a protective field that prevents status problems for five turns.";
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
        isProtectMove = true;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int guard = battleScreen.FieldEffects.Safeguard.Self;
        if (own == false)
        {
            guard = battleScreen.FieldEffects.Safeguard.Opponent;
        }

        if (guard == 0)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.Safeguard.Self = 5;

                battleScreen.BattleQuery.Add(new TextQueryObject("Your team became cloaked in a mystical veil!"));
            }
            else
            {
                battleScreen.FieldEffects.Safeguard.Opponent = 5;

                battleScreen.BattleQuery.Add(new TextQueryObject("The other team became cloaked in a mystical veil!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
