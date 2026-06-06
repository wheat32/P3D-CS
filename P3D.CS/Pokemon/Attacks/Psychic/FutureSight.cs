using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class FutureSight : Attack
{
    public FutureSight()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 248;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 120;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Future Sight");
        Description = "Two turns after this move is used, a hunk of psychic energy attacks the target.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
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
        useEffectiveness = true;
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

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
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
            if (battleScreen.FieldEffects.FutureSightTurns.Self == 0)
            {
                battleScreen.FieldEffects.FutureSightTurns.Self = 3;
                battleScreen.FieldEffects.FutureSightID.Self = 0;
                battleScreen.FieldEffects.FutureSightDamage.Self = base.GetDamage(false, own, own == false, battleScreen);

                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " foresaw an attack!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.FutureSightTurns.Opponent == 0)
            {
                battleScreen.FieldEffects.FutureSightTurns.Opponent = 3;
                battleScreen.FieldEffects.FutureSightID.Opponent = 0;
                battleScreen.FieldEffects.FutureSightDamage.Opponent = base.GetDamage(false, own, own == false, battleScreen);

                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " foresaw an attack!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
