using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Poison;

public class ClearSmog : Attack
{
    public ClearSmog()
    {
        // #Definitions
        type = new Element(Element.Types.Poison);
        ID = 499;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 50;
        Accuracy = 0;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Clear Smog");
        Description = "The user attacks by throwing a clump of special mud. All stat changes are returned to normal.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
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
        isWonderGuardAffected = false;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {

            battleScreen.SelfPokemon.StatAttack = 0;
            battleScreen.SelfPokemon.StatDefense = 0;
            battleScreen.SelfPokemon.StatSpAttack = 0;
            battleScreen.SelfPokemon.StatSpDefense = 0;
            battleScreen.SelfPokemon.StatSpeed = 0;
            battleScreen.SelfPokemon.Accuracy = 0;
            battleScreen.SelfPokemon.Evasion = 0;


            battleScreen.OpponentPokemon.StatAttack = 0;
            battleScreen.OpponentPokemon.StatDefense = 0;
            battleScreen.OpponentPokemon.StatSpAttack = 0;
            battleScreen.OpponentPokemon.StatSpDefense = 0;
            battleScreen.OpponentPokemon.StatSpeed = 0;
            battleScreen.OpponentPokemon.Accuracy = 0;
            battleScreen.OpponentPokemon.Evasion = 0;

        battleScreen.BattleQuery.Add(new TextQueryObject("All stat changes were eliminated!"));
    }

}
