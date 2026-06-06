using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Endeavor : Attack
{
    public Endeavor()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 283;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Endeavor");
        Description = "An attack move that cuts down the target's HP to equal the user's HP.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
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
    }

    public override int GetDamage(bool critical, bool own, bool targetPokemon, BattleScreen battleScreen, String extraParameter = "", Attack typeEffectivenessAttack = null)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon o = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            o = battleScreen.SelfPokemon;
        }

        if (o.HP <= p.HP)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return 0;
        }

        return (o.HP - p.HP);
    }

}
