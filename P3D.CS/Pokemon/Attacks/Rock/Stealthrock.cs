using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class StealthRock : Attack
{
    public StealthRock()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 446;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Stealth Rock");
        Description = "The user lays a trap of levitating stones around the opposing team. The trap hurts opposing Pokémon that switch into battle.";
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
        int stealthrock = 0;
        if (own == true)
        {
            stealthrock = battleScreen.FieldEffects.StealthRock.Self;
        }
        else
        {
            stealthrock = battleScreen.FieldEffects.StealthRock.Opponent;
        }
        if (stealthrock < 1)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.StealthRock.Self += 1;
                battleScreen.BattleQuery.Add(new TextQueryObject("Pointed stones float in the air around the opposing team!"));
            }
            else
            {
                battleScreen.FieldEffects.StealthRock.Opponent += 1;
                battleScreen.BattleQuery.Add(new TextQueryObject("Pointed stones float in the air around your team!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
