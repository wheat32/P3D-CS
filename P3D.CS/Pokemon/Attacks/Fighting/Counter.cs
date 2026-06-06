using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class Counter : Attack
{
    public Counter()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 68;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Counter");
        Description = "A retaliation move that counters any physical attack, inflicting double the damage taken.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = -5;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = false;
        kingsrockAffected = false;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = false;
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

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        bool hasBeenDamaged = battleScreen.FieldEffects.PokemonDamagedLastTurn.Opponent;
        if (own == true)
        {
            hasBeenDamaged = battleScreen.FieldEffects.PokemonDamagedLastTurn.Self;
        }

        if (battleScreen.FieldEffects.TurnCounts.Opponent == 0 || battleScreen.FieldEffects.TurnCounts.Self == 0)
        {
            if (own == false)
            {
                hasBeenDamaged = battleScreen.FieldEffects.PokemonDamagedThisTurn.Opponent;
            }
            else
            {
                hasBeenDamaged = battleScreen.FieldEffects.PokemonDamagedThisTurn.Self;
            }
        }

        if (hasBeenDamaged == true)
        {
            int damage = battleScreen.FieldEffects.LastDamage.Self;
            if (own == true)
            {
                damage = battleScreen.FieldEffects.LastDamage.Opponent;
            }

            if (damage > 0)
            {
                Attack lastMove = battleScreen.FieldEffects.LastMove.Self;
                if (own == true)
                {
                    lastMove = battleScreen.FieldEffects.LastMove.Opponent;
                }
                if (lastMove != null)
                {
                    if (lastMove.counterAffected == true)
                    {
                        return false;
                    }
                }
            }
        }

        battleScreen.BattleQuery.Add(new TextQueryObject("But it failed!"));
        return true;
    }

    public override int GetDamage(bool critical, bool own, bool targetPokemon, BattleScreen battleScreen, String extraParameter = "", Attack typeEffectivenessAttack = null)
    {
        int damage = battleScreen.FieldEffects.LastDamage.Self;
        if (own == true)
        {
            damage = battleScreen.FieldEffects.LastDamage.Opponent;
        }

        return damage * 2;
    }

}
