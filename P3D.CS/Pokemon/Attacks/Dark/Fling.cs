using Microsoft.Xna.Framework;
using P3D.Items;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class Fling : Attack
{
    public Fling()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 374;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Fling");
        Description = "The user flings its held item at the target to attack. This move's power and effects depend on the item.";
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

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.Item == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.Item == null)
        {
            return 10;
        }
        else
        {
            return p.Item.FlingDamage;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }
        if (p.Item.IsMegaStone == true)
        {
            return;
        }
        if (p.Item != null)
        {
            // Clear prior effect chances to add the chance depending on the item.
            effectChances.Clear();

            switch (p.Item.OriginalName.ToLower())
            {
                case "flame orb":
                    effectChances.Add(30);
                    if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
                    {
                        battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:fling");
                    }
                    break;
                case "king's rock":
                case "razor fang":
                    effectChances.Add(30);
                    if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
                    {
                        battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:fling");
                    }
                    break;
                case "light ball":
                    effectChances.Add(30);
                    if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
                    {
                        battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:fling");
                    }
                    break;
                case "mental herb":
                    if (p.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation) == true)
                    {
                        effectChances.Add(10);
                        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
                        {
                            p.RemoveVolatileStatus(Pokemon.VolatileStatus.Infatuation);
                            battleScreen.BattleQuery.Add(new TextQueryObject("Cured the infatuation of " + p.GetDisplayName() + "."));
                        }
                    }
                    break;
                case "poison barb":
                    effectChances.Add(70);
                    if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
                    {
                        battleScreen.Battle.InflictPoison(own == false, own, battleScreen, false, "", "move:fling");
                    }
                    break;
                case "toxic orb":
                    effectChances.Add(30);
                    if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
                    {
                        battleScreen.Battle.InflictPoison(own == false, own, battleScreen, true, "", "move:fling");
                    }
                    break;
                case "white herb":
                    effectChances.Add(10);
                    if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
                    {
                        p.StatAttack = 0;
                        p.StatDefense = 0;
                        p.StatSpAttack = 0;
                        p.StatSpDefense = 0;
                        p.StatSpeed = 0;
                        p.Accuracy = 0;
                        p.Evasion = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject("Restored the stats of " + p.GetDisplayName() + "."));
                    }
                    break;
            }

            String ItemID = String.Empty;
            if (p.Item.IsGameModeItem == true)
            {
                ItemID = p.Item.gmID;
            }
            else
            {
                ItemID = p.Item.ID.ToString();
            }
            p.OriginalItem = Item.GetItemByID(ItemID);
            p.Item = null;
        }
    }

}
