using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class Reflect : Attack
{
    public Reflect()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 115;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Reflect");
        Description = "A wondrous wall is put up to suppress damage from physical attacks for five turns.";
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
            if (battleScreen.FieldEffects.Reflect.Self == 0)
            {
                battleScreen.FieldEffects.Reflect.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject("Reflect raised your team's Defense!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Reflect.Opponent == 0)
            {
                battleScreen.FieldEffects.Reflect.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject("Reflect raised the other team's Defense!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
