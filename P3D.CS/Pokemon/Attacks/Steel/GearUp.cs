using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class GearUp : Attack
{
    public GearUp()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 674;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Gear Up");
        Description = "The user engages its gears to raise the Attack and Sp. Atk. stats of ally Pokémon with the Plus or Minus Ability.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllAllies;
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.RaiseAttack;
        aiField2 = AIField.RaiseSpAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }
        // Plus and Minus
        if (p.Ability.ID == 57 || p.Ability.ID == 58)
        {
            bool a = battleScreen.Battle.RaiseStat(own, own, battleScreen, "Attack", 1, "", "move:gearup");
            bool b = battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Attack", 1, "", "move:gearup");

            if (a == false && b == false)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
