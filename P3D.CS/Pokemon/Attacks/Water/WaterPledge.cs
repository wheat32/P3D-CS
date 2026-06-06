using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class WaterPledge : Attack
{
    public WaterPledge()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 518;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 80;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Water Pledge");
        Description = "A column of water strikes the target. When combined with its fire equivalent, the damage increases and a rainbow appears.";
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
                case "fire pledge":
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
                case "fire pledge":
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
                        battleScreen.FieldEffects.GrassPledge.Self = 4;
                    }
                    else
                    {
                        battleScreen.FieldEffects.GrassPledge.Opponent = 4;
                    }

                    battleScreen.BattleQuery.Add(new TextQueryObject("A swamp enveloped the other team!"));
                    break;
                case "fire pledge":
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
