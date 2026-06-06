using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class Trick : Attack
{
    public Trick()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 271;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Trick");
        Description = "The user catches the target off guard and swaps its held item with its own.";
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
            if (p.Item != null && p.OriginalItem == null)
            {
                p.OriginalItem = p.Item;
            }
            if (op.Item != null && op.OriginalItem == null)
            {
                op.OriginalItem = op.Item;
            }

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

            if (battleScreen.FieldEffects.StolenFromOpponentItems.ContainsKey(battleScreen.OpponentPokemonIndex) == false)
            {
                Item item = null;
                if (own == true)
                {
                    item = i2;
                }
                else
                {
                    item = i1;
                }
                if (item != null)
                {
                    battleScreen.FieldEffects.StolenFromOpponentItems.Add(battleScreen.OpponentPokemonIndex, item);
                }
            }

            if (battleScreen.FieldEffects.StolenFromSelfItems.ContainsKey(battleScreen.SelfPokemonIndex) == false)
            {
                Item item = null;
                if (own == true)
                {
                    item = i1;
                }
                else
                {
                    item = i2;
                }
                if (item != null)
                {
                    battleScreen.FieldEffects.StolenFromSelfItems.Add(battleScreen.SelfPokemonIndex, item);
                }
            }

            if (p.Item != null && p.OriginalItem != null)
            {
                String pItemID = "";
                if (p.Item.IsGameModeItem == true)
                {
                    pItemID = p.Item.gmID;
                }
                else
                {
                    pItemID = p.Item.ID.ToString();
                }
                String pOriginalItemID = "";
                if (p.OriginalItem.IsGameModeItem == true)
                {
                    pOriginalItemID = p.OriginalItem.gmID;
                }
                else
                {
                    pOriginalItemID = p.OriginalItem.ID.ToString();
                }

                if (pItemID == pOriginalItemID && p.Item.AdditionalData == p.OriginalItem.AdditionalData)
                {
                    p.OriginalItem = null;

                    if (own == true)
                    {
                        if (battleScreen.FieldEffects.StolenFromSelfItems.ContainsKey(battleScreen.SelfPokemonIndex))
                        {
                            battleScreen.FieldEffects.StolenFromSelfItems.Remove(battleScreen.SelfPokemonIndex);
                        }
                    }
                    else
                    {
                        if (battleScreen.FieldEffects.StolenFromOpponentItems.ContainsKey(battleScreen.OpponentPokemonIndex))
                        {
                            battleScreen.FieldEffects.StolenFromOpponentItems.Remove(battleScreen.OpponentPokemonIndex);
                        }
                    }
                }
            }
            if (op.Item != null && op.OriginalItem != null)
            {
                String opItemID = "";
                if (op.Item.IsGameModeItem == true)
                {
                    opItemID = op.Item.gmID;
                }
                else
                {
                    opItemID = op.Item.ID.ToString();
                }
                String opOriginalItemID = "";
                if (op.OriginalItem.IsGameModeItem == true)
                {
                    opOriginalItemID = op.OriginalItem.gmID;
                }
                else
                {
                    opOriginalItemID = op.OriginalItem.ID.ToString();
                }

                if (opItemID == opOriginalItemID && op.Item.AdditionalData == op.OriginalItem.AdditionalData)
                {
                    op.OriginalItem = null;
                    if (own == false)
                    {
                        if (battleScreen.FieldEffects.StolenFromSelfItems.ContainsKey(battleScreen.SelfPokemonIndex))
                        {
                            battleScreen.FieldEffects.StolenFromOpponentItems.Remove(battleScreen.SelfPokemonIndex);
                        }
                    }
                    else
                    {
                        if (battleScreen.FieldEffects.StolenFromOpponentItems.ContainsKey(battleScreen.OpponentPokemonIndex))
                        {
                            battleScreen.FieldEffects.StolenFromOpponentItems.Remove(battleScreen.OpponentPokemonIndex);
                        }
                    }
                }
            }

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " switched items with " + op.GetDisplayName() + "."));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
