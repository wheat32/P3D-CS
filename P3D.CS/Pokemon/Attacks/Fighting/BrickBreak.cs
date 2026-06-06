using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class BrickBreak : Attack
{
    public BrickBreak()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 280;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 75;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Brick Break");
        Description = "The user attacks with a swift chop. It can also break barriers, such as Light Screen and Reflect.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
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
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.RemoveReflectLightscreen;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.SelfPokemon;
        }

        if (p.IsType(Element.Types.Ghost) == false)
        {
            if (own == true)
            {
                if (battleScreen.FieldEffects.LightScreen.Opponent > 0)
                {
                    battleScreen.FieldEffects.LightScreen.Opponent = 0;
                    battleScreen.BattleQuery.Add(new TextQueryObject("The Light Screen wore off!"));
                }
                if (battleScreen.FieldEffects.Reflect.Opponent > 0)
                {
                    battleScreen.FieldEffects.Reflect.Opponent = 0;
                    battleScreen.BattleQuery.Add(new TextQueryObject("The Reflect wore off!"));
                }
            }
            else
            {
                if (battleScreen.FieldEffects.LightScreen.Self > 0)
                {
                    battleScreen.FieldEffects.LightScreen.Self = 0;
                    battleScreen.BattleQuery.Add(new TextQueryObject("The Light Screen wore off!"));
                }
                if (battleScreen.FieldEffects.Reflect.Self > 0)
                {
                    battleScreen.FieldEffects.Reflect.Self = 0;
                    battleScreen.BattleQuery.Add(new TextQueryObject("The Reflect wore off!"));
                }
            }
        }
    }

}
