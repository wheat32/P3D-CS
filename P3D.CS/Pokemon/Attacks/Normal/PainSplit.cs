using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class PainSplit : Attack
{
    public PainSplit()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 220;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Pain Split");
        Description = "The user adds its HP to the target's HP, then equally shares the combined HP with the target.";
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

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

        int totalHP = p.HP + op.HP;
        int newHP = (int)(Math.Ceiling((double)(totalHP / 2)));

        bool failed = true;

        if (p.HP < newHP)
        {
            battleScreen.Battle.GainHP(newHP - p.HP, own, own, battleScreen, "", "move:painsplit");
            failed = false;
        }
        else if (p.HP > newHP)
        {
            battleScreen.Battle.ReduceHP(p.HP - newHP, own, own, battleScreen, "", "move:painsplit");
            failed = false;
        }

        if (op.HP < newHP)
        {
            battleScreen.Battle.GainHP(newHP - op.HP, own == false, own, battleScreen, "", "move:painsplit");
            failed = false;
        }
        else if (op.HP > newHP)
        {
            battleScreen.Battle.ReduceHP(op.HP - newHP, own == false, own, battleScreen, "", "move:painsplit");
            failed = false;
        }

        if (failed == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The battlers shared their pain."));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
