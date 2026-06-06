using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class Charge : Attack
{
    public Charge()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 268;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Charge");
        Description = "The user boosts the power of the Electric move it uses on the next turn. It also raises the user's Sp. Def. stat.";
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

        aiField1 = AIField.RaiseSpDefense;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.Charge.Self = 2;
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " began charging power."));
        }
        else
        {
            battleScreen.FieldEffects.Charge.Opponent = 2;
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " began charging power."));
        }

        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Defense", 1, "", "move:charge");
    }

}
