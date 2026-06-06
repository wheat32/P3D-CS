using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class Aromatherapy : Attack
{
    public Aromatherapy()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 312;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Aromatherapy");
        Description = "The user releases a soothing scent that heals all status conditions affecting the user's party.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllOwn;
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

        aiField1 = AIField.CureStatus;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        List<Pokemon> healed = [];

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.Status != Pokemon.StatusProblems.None && p.Status != Pokemon.StatusProblems.Fainted)
        {
            if (battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
            {
                healed.Add(p);
            }
        }

        if (own == true)
        {
            foreach (Pokemon tp in Core.Player.Pokemons)
            {
                if (tp.Equals(p) == false)
                {
                    if (tp.Status != Pokemon.StatusProblems.None && tp.Status != Pokemon.StatusProblems.Fainted)
                    {
                        if (battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
                        {
                            healed.Add(tp);
                        }
                    }
                }
            }
        }
        else
        {
            if (battleScreen.IsTrainerBattle == true || battleScreen.IsPVPBattle == true || battleScreen.IsRemoteBattle == true)
            {
                foreach (Pokemon tp in battleScreen.Trainer.Pokemons)
                {
                    if (tp.Equals(p) == false)
                    {
                        if (tp.Status != Pokemon.StatusProblems.None && tp.Status != Pokemon.StatusProblems.Fainted)
                        {
                            if (battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
                            {
                                healed.Add(tp);
                            }
                        }
                    }
                }
            }
        }

        battleScreen.BattleQuery.Add(new TextQueryObject("A soothing aroma wafted through the area!"));

        if (healed.Count > 0)
        {
            for (int i = 0; i <= healed.Count - 1; i++)
            {
                String statusName = "poisoning";
                switch (healed[i].Status)
                {
                    case Pokemon.StatusProblems.BadPoison:
                    case Pokemon.StatusProblems.Poison:
                        statusName = "poisong";
                        break;
                    case Pokemon.StatusProblems.Burn:
                        statusName = "burn";
                        break;
                    case Pokemon.StatusProblems.Freeze:
                        statusName = "freezing";
                        break;
                    case Pokemon.StatusProblems.Paralyzed:
                        statusName = "paralyzis";
                        break;
                }

                healed[i].Status = Pokemon.StatusProblems.None;
                battleScreen.BattleQuery.Add(new TextQueryObject(healed[i].GetDisplayName() + " was cured of its " + statusName + "!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
