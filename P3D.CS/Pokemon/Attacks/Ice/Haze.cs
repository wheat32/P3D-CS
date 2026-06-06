using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class Haze : Attack
{
    public Haze()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 114;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Haze");
        Description = "The user creates a haze that eliminates every stat change among all the Pokémon engaged in battle.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllTargets;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
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
