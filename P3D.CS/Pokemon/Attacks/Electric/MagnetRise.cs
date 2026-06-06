using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class MagnetRise : Attack
{
    public MagnetRise()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 393;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Magnet Rise");
        Description = "The user levitates using electrically generated magnetism for five turns.";
        criticalChance = 1;
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
            if (battleScreen.FieldEffects.MagnetRise.Self == 0)
            {
                battleScreen.FieldEffects.MagnetRise.Self = 5;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " levitated on electromagnetism!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("but it failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.MagnetRise.Opponent == 0)
            {
                battleScreen.FieldEffects.MagnetRise.Opponent = 5;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " levitated on electromagnetism!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("but it failed!"));
            }
        }
    }

}
