using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class TriAttack : Attack
{
    public TriAttack()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 161;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 80;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Tri Attack");
        Description = "The user strikes with a simultaneous three-beam attack. May also burn, freeze, or leave the target with paralysis.";
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
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = true;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        effectChances.Add(20);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            int r = Core.Random.Next(0, 3);
            switch (r)
            {
                case 0:
                    battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:triattack");
                    break;
                case 1:
                    battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:triattack");
                    break;
                case 2:
                    battleScreen.Battle.InflictFreeze(own == false, own, battleScreen, "", "move:triattack");
                    break;
            }
        }
    }

}
