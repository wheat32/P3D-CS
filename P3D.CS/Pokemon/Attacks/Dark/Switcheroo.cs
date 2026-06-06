using Microsoft.Xna.Framework;
using P3D.Items;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class Switcheroo : Attack
{
    public Switcheroo()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 415;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Switcheroo");
        Description = "The user trades held items with the target faster than the eye can follow.";
        criticalChance = 0;
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
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
        bool CanSwitchItems = true;
        if (p.Item == null && op.Item == null)
        {
            CanSwitchItems = false;
        }
        if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) && op.Ability.Name.ToLower() == "sticky hold")
        {
            CanSwitchItems = false;
        }
        if (p.Item != null && p.Item.OriginalName.ToLower() == "griseous orb" && p.Number == 487)
        {
            CanSwitchItems = false;
        }
        if (op.Item != null && op.Item.OriginalName.ToLower() == "griseous orb" && op.Number == 487)
        {
            CanSwitchItems = false;
        }
        if (p.Item != null && p.Item.OriginalName.ToLower().EndsWith(" drive") == true && p.Number == 649)
        {
            CanSwitchItems = false;
        }
        if (op.Item != null && op.Item.OriginalName.ToLower().EndsWith(" drive") == true && op.Number == 649)
        {
            CanSwitchItems = false;
        }
        if (p.Item != null && p.Item.OriginalName.ToLower().EndsWith(" plate") == true && p.Number == 493)
        {
            CanSwitchItems = false;
        }
        if (op.Item != null && op.Item.OriginalName.ToLower().EndsWith(" plate") == true && op.Number == 493)
        {
            CanSwitchItems = false;
        }
        if (p.Item != null && p.Item.OriginalName.ToLower().EndsWith(" memory") == true && p.Number == 773)
        {
            CanSwitchItems = false;
        }
        if (op.Item != null && op.Item.OriginalName.ToLower().EndsWith(" memory") == true && op.Number == 773)
        {
            CanSwitchItems = false;
        }
        if (p.Item != null && p.Item.IsMail == true)
        {
            CanSwitchItems = false;
        }
        if (op.Item != null && op.Item.IsMail == true)
        {
            CanSwitchItems = false;
        }
        if ((p.Item != null && p.Item.IsMegaStone) || (op.Item != null && op.Item.IsMegaStone))
        {
            CanSwitchItems = false;
        }

        if (CanSwitchItems)
        {
            Item i1 = null;
            Item i2 = null;
            if (p.Item != null)
            {
                i1 = p.Item;
            }
            if (op.Item != null)
            {
                i2 = op.Item;
            }
            p.Item = i2;
            op.Item = i1;
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " switched items with " + op.GetDisplayName() + "."));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
