using Microsoft.Xna.Framework;
using P3D.Items;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Bestow : Attack
{
    public Bestow()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 516;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Bestow");
        Description = "The user passes its held item to the target when the target isn't holding an item.";
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
        useAccEvasion = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.CannotMiss;
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

        bool a = false;
        bool b = false;

        if (p.Item != null)
        {
            a = true;
            if (p.Item.IsMegaStone == true)
            {
                b = true;
            }
            if (p.Ability.Name.ToLower() == "multitype" && p.Item.OriginalName.ToLower().EndsWith(" plate"))
            {
                b = true;
            }
            if (p.Ability.Name.ToLower() == "rks system" && p.Item.OriginalName.ToLower().EndsWith(" memory"))
            {
                b = true;
            }
            // Giratina
            if (p.Item.OriginalName.ToLower() == "griseous orb" && p.Number == 487)
            {
                b = true;
            }
            // Genesect
            if (p.Item.OriginalName.ToLower().EndsWith(" drive") == true && p.Number == 649)
            {
                b = true;
            }
            if (p.Item.IsMail == true)
            {
                b = true;
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return;
        }

        if (op.Item == null && a == true && b == false)
        {
            Item item = p.Item;
            if (p.OriginalItem == null)
            {
                p.OriginalItem = item;
            }

            if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, op.GetDisplayName() + " received the item " + p.Item.OneLineName() + " from " + p.GetDisplayName() + "!", "move:bestow"))
            {
                if (own == false && battleScreen.FieldEffects.StolenFromOpponentItems.ContainsKey(battleScreen.OpponentPokemonIndex) == false)
                {
                    battleScreen.FieldEffects.StolenFromOpponentItems.Add(battleScreen.OpponentPokemonIndex, item);
                }
                op.Item = item;

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
            }
            else
            {

                if (p.OriginalItem.ID == p.Item.ID && p.OriginalItem.AdditionalData == p.Item.AdditionalData)
                {
                    p.OriginalItem = null;
                }
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }

    }

}
