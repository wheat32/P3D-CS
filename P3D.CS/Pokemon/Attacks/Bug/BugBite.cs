using Microsoft.Xna.Framework;
using P3D.Items;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Bug;

public class BugBite : Attack
{
    public BugBite()
    {
        // #Definitions
        type = new Element(Element.Types.Bug);
        ID = 450;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 60;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Bug Bite");
        Description = "The user bites the target. If the target is holding a Berry, the user eats it and gains its effect.";
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
        counterAffected = false;

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
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        if (op.Item != null)
        {
            if (op.Item.IsBerry == true)
            {
                String ItemID = String.Empty;
                if (op.Item.IsGameModeItem == true)
                {
                    ItemID = op.Item.gmID;
                }
                else
                {
                    ItemID = op.Item.ID.ToString();
                }

                battleScreen.Battle.RemoveHeldItem(own == false, own, battleScreen, p.GetDisplayName() + " ate the " + op.Item.OneLineName() + " berry!", "move:bugbite");
                battleScreen.Battle.UseBerry(own, own, Item.GetItemByID(ItemID.ToString()), battleScreen, "", "move:bugbite");
            }
        }
    }

}
