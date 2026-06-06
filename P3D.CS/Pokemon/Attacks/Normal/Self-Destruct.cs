using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class SelfDestruct : Attack
{
    public SelfDestruct()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 120;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 200;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Self-Destruct");
        Description = "The user attacks everything around it by causing an explosion. The user faints upon using this move.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllTargets;
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
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;

        isExplosiveMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Selfdestruct;
    }

    public override void PreAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        battleScreen.Battle.ReduceHP(p.HP, own, own, battleScreen, p.GetDisplayName() + " exploded!", "move:selfdestruct");
    }

}
