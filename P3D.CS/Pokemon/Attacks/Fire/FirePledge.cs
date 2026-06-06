using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class FirePledge : Attack
{
    public FirePledge()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 519;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 80;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Fire Pledge");
        Description = "A column of fire hits opposing Pokémon. When used with its Grass equivalent, its damage increases into a vast sea of fire.";
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
        hasSecondaryEffect = true;
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

    public override void BeforeDealingDamage(bool own, BattleScreen battleScreen)
    {
        Attack lastMove = battleScreen.FieldEffects.LastMove.Self;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        }

        if (lastMove != null)
        {
            switch (lastMove.Name.ToLower())
            {
                case "grass pledge":
                case "water pledge":
                    battleScreen.BattleQuery.Add(new TextQueryObject("The two moves are joined! It's a combined move!"));
                    break;
            }
        }
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Attack lastMove = battleScreen.FieldEffects.LastMove.Self;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        }

        if (lastMove != null)
        {
            switch (lastMove.Name.ToLower())
            {
                case "grass pledge":
                    return Power * 2;
                    break;
                case "water pledge":
                    return Power * 2;
                    break;
            }
        }

        return Power;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Attack lastMove = battleScreen.FieldEffects.LastMove.Self;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        }

        if (lastMove != null)
        {
            switch (lastMove.Name.ToLower())
            {
                case "grass pledge":
                    if (own == true)
                    {
                        battleScreen.FieldEffects.FirePledge.Self = 4;
                    }
                    else
                    {
                        battleScreen.FieldEffects.FirePledge.Opponent = 4;
                    }

                    battleScreen.BattleQuery.Add(new TextQueryObject("A sea of fire enveloped the other team!"));
                    break;
                case "water pledge":
                    if (own == true)
                    {
                        battleScreen.FieldEffects.WaterPledge.Self = 4;
                    }
                    else
                    {
                        battleScreen.FieldEffects.WaterPledge.Opponent = 4;
                    }

                    battleScreen.BattleQuery.Add(new TextQueryObject("A rainbow appeared in the sky!"));
                    break;
            }
        }
    }

}
