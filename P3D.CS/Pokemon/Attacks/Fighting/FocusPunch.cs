using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class FocusPunch : Attack
{
    public FocusPunch()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 264;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 150;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Focus Punch");
        Description = "The user focuses its mind before launching a punch. It will fail if the user is hit before it is used.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = -3;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;
        isPunchingMove = true;
        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.PokemonDamagedThisTurn.Self == true)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " lost its focus && couldn't move!"));
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.PokemonDamagedThisTurn.Opponent == true)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " lost its focus && couldn't move!"));
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
