using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Recover : Attack
{
    public Recover()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 105;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Recover");
        Description = "Restoring its own cells, the user restores its own HP by half of its max HP.";
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = true;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Healing;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int restoreHP = (int)(Math.Ceiling((double)(p.MaxHP / 2)));

        if (restoreHP > 0 && p.HP < p.MaxHP && p.HP > 0)
        {
            battleScreen.Battle.GainHP(restoreHP, own, own, battleScreen, p.GetDisplayName() + "'s HP was restored!", "move:recover");
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
