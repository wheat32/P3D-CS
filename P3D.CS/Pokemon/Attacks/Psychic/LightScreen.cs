using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class LightScreen : Attack
{
    public LightScreen()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 113;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Light Screen");
        Description = "A wondrous wall of light is put up to suppress damage from special attacks for five turns.";
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
        int turns = BattleCalculation.FieldEffectTurns(battleScreen, own, Name.ToLower());
        if (own == true)
        {
            if (battleScreen.FieldEffects.LightScreen.Self == 0)
            {
                battleScreen.FieldEffects.LightScreen.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject("Light Screen raised your team's Special Defense!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.LightScreen.Opponent == 0)
            {
                battleScreen.FieldEffects.LightScreen.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject("Light Screen raised the other team's Special Defense!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
