using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class Ingrain : Attack
{
    public Ingrain()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 275;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Ingrain");
        Description = "The user lays roots that restore its HP on every turn. Because it is rooted, it can't switch out.";
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.Ingrain.Self == 0)
            {
                battleScreen.FieldEffects.Ingrain.Self = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " planted its roots!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Ingrain failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Ingrain.Opponent == 0)
            {
                battleScreen.FieldEffects.Ingrain.Opponent = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " planted its roots!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Ingrain failed!"));
            }
        }
    }

}
