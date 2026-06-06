using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Recycle : Attack
{
    public Recycle()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 278;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Recycle");
        Description = "The user recycles a held item that has been used in battle so it can be used again.";
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

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            Pokemon p = battleScreen.SelfPokemon;
            if (battleScreen.FieldEffects.ConsumedItem.Self != null)
            {
                p.Item = battleScreen.FieldEffects.ConsumedItem.Self;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " found one " + p.Item.OneLineName() + "!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Recycle failed!"));
            }
        }
        else
        {
            Pokemon p = battleScreen.OpponentPokemon;

            if (battleScreen.FieldEffects.ConsumedItem.Opponent != null)
            {
                p.Item = battleScreen.FieldEffects.ConsumedItem.Opponent;
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " found one " + p.Item.OneLineName() + "!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Recycle failed!"));
            }
        }
    }

}
