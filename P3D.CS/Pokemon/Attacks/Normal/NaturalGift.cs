using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class NaturalGift : Attack
{
    public NaturalGift()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 363;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Natural Gift");
        Description = "The user draws power to attack by using its held Berry. The Berry determines the move's type and power.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = false;

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
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int itemID = 0;
        if (p.Item != null && p.Item.IsGameModeItem == false)
        {
            itemID = p.Item.ID;
        }

        if (1999 < itemID && itemID < 2016)
        {
            return 80;
        }
        else if (2034 < itemID && itemID < 2052)
        {
            return 80;
        }
        else if (2063 < itemID && itemID < 2065)
        {
            return 80;
        }
        else if (2015 < itemID && itemID < 2032)
        {
            return 90;
        }
        else if (2031 < itemID && itemID < 2035)
        {
            return 100;
        }
        else if (2051 < itemID && itemID < 2064)
        {
            return 100;
        }
        else if (2064 < itemID && itemID < 2067)
        {
            return 100;
        }
        else
        {
            return 0;
        }
    }

    public override Element GetAttackType(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.Item != null)
        {
            if (p.Item.isBerry == true)
            {
                return BattleSystem.GameModeElementLoader.GetElementByID((Items.Berry)p.Item.Type);
            }
        }

        return new Element(Element.Types.Normal);
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        if (p.Item == null)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }
        else
        {
            if (p.Item.isBerry == false)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                return true;
            }
        }

        if (op.Ability.Name.ToLower() == "unnerve" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }

        if (battleScreen.FieldEffects.CanUseItem(own) == false || battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }

        return false;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "", "move:naturalgift");
    }

}
