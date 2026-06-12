using Microsoft.Xna.Framework;
using P3D.Items;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Covet : Attack
{
    public Covet()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 343;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 60;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Covet");
        Description = "The user endearingly approaches the target, then steals the target's held item.";
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
        if (op.Item.OriginalName.ToLower().EndsWith(" drive") == true && op.Number == 649)
        {
            return;
        }
        if (op.Item.IsMail == true)
        {
            return;
        }

        if (p.Item == null)
        {
            Item item = op.Item;
            if (op.OriginalItem == null)
            {
                op.OriginalItem = item;
            }

            if (battleScreen.Battle.RemoveHeldItem(own == false, own, battleScreen, "Covet stole the item " + op.Item.OneLineName() + " from " + op.GetDisplayName() + "!", "move:covet", true))
            {
                if (own == true && battleScreen.FieldEffects.StolenFromOpponentItems.ContainsKey(battleScreen.OpponentPokemonIndex) == false)
                {
                    battleScreen.FieldEffects.StolenFromOpponentItems.Add(battleScreen.OpponentPokemonIndex, item);
                }
                p.Item = item;

                if (p.Item != null && p.OriginalItem != null)
                {
                    String pItemID = String.Empty;
                    if (p.Item.IsGameModeItem == true)
                    {
                        pItemID = p.Item.gmID;
                    }
                    else
                    {
                        pItemID = p.Item.ID.ToString();
                    }
                    String pOriginalItemID = String.Empty;
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

}
