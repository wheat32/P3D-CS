using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class KnockOff : Attack
{
    public KnockOff()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 282;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 65;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Knock Off");
        Description = "The user slaps down the target's held item, and that item can't be used in that battle. The move does more damage if the target has a held item.";
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
        kingsrockAffected = false;
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
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        // Conditions
        if (op.Item == null)
        {
            return Power;
        }
        if (op.Item.IsMegaStone == true)
        {
            return Power;
        }
        if (op.Ability.Name.ToLower() == "multitype" && op.Item.OriginalName.ToLower().EndsWith(" plate"))
        {
            return Power;
        }
        if (op.Ability.Name.ToLower() == "rks system" && op.Item.OriginalName.ToLower().EndsWith(" memory"))
        {
            return Power;
        }
        if (op.Item.OriginalName.ToLower() == "griseous orb" && op.Number == 487)
        {
            return Power;
        }
        if (op.Item.OriginalName.ToLower().EndsWith(" drive") == true && op.Number == 649)
        {
            return Power;
        }
        if (op.Item.IsMail == true)
        {
            return Power;
        }

        return (int)(Power * 1.5F);
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

        // Conditions
        if (op.Item == null)
        {
            return;
        }
        if (op.Item.IsMegaStone == true)
        {
            return;
        }
        if (op.Ability.Name.ToLower() == "multitype" && op.Item.OriginalName.ToLower().EndsWith(" plate"))
        {
            return;
        }
        if (op.Ability.Name.ToLower() == "rks system" && op.Item.OriginalName.ToLower().EndsWith(" memory"))
        {
            return;
        }
        if (op.Item.OriginalName.ToLower() == "griseous orb" && op.Number == 487)
        {
            return;
        }
        if (op.Item.OriginalName.ToLower().EndsWith(" drive") == true && p.Number == 649)
        {
            return;
        }
        if (op.Item.IsMail == true)
        {
            return;
        }

        Item item = op.Item;
        if (op.OriginalItem == null)
        {
            op.OriginalItem = item;
        }

        if (battleScreen.Battle.RemoveHeldItem(own == false, own, battleScreen, p.GetDisplayName() + " knocked off the " + op.GetDisplayName() + "'s " + op.OriginalItem.OneLineName() + "!", "move:knockoff") == true)
        {
            if (own == false && battleScreen.FieldEffects.StolenFromSelfItems.ContainsKey(battleScreen.SelfPokemonIndex) == false)
            {
                battleScreen.FieldEffects.StolenFromSelfItems.Add(battleScreen.SelfPokemonIndex, item);
            }
        }
        else
        {
            if (op.OriginalItem.ID == op.Item.ID && op.OriginalItem.AdditionalData == op.Item.AdditionalData)
            {
                op.OriginalItem = null;
            }
        }

    }

}
