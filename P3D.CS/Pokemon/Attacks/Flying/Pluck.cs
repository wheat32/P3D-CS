using Microsoft.Xna.Framework;
using P3D.Items;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class Pluck : Attack
{
    public Pluck()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 365;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 60;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Pluck");
        Description = "The user pecks the target. If the target is holding a Berry, the user eats it and gains its effect.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneTarget;
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
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        if (op.Item != null)
        {
            if (op.Item.IsBerry == true)
            {
                String ItemID = "";
                if (op.Item.IsGameModeItem)
                {
                    ItemID = op.Item.gmID;
                }
                else
                {
                    ItemID = op.Item.ID.ToString();
                }

                battleScreen.Battle.UseBerry(own, own, Item.GetItemByID(ItemID), battleScreen, "", "move:pluck");
            }
        }
    }

}
