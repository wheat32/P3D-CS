using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Substitute : Attack
{
    public Substitute()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 164;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Substitute");
        Description = "The user makes a copy of itself using some of its HP. The copy serves as the user’s decoy.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
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
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool fails = false;
        int looseHP = 0;

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.MaxHP == 1 || p.HP <= (int)(Math.Floor((double)(p.MaxHP / 4))))
        {
            fails = true;
        }
        else
        {
            if (p.MaxHP > 3)
            {
                looseHP = (int)(Math.Floor((double)(p.MaxHP / 4)));
            }
        }

        if (looseHP > 0 && fails == false)
        {
            if (looseHP >= p.HP)
            {
                looseHP -= 1;
            }
        }

        if (fails == false)
        {
            battleScreen.Battle.ReduceHP(looseHP, own, own, battleScreen, p.GetDisplayName() + " put in a substitute!", "move:substitute");
            if (own == true)
            {
                battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.SelfPokemon, "Substitute", 0, 1, -1, -1));
                battleScreen.FieldEffects.Substitute.Self = looseHP + 1;
            }
            else
            {
                battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(false, ToggleEntityQueryObject.BattleEntities.SelfPokemon, "Substitute", 0, 1, -1, -1));
                battleScreen.FieldEffects.Substitute.Opponent = looseHP + 1;
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
