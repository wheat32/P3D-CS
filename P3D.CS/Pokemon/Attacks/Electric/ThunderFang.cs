using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class ThunderFang : Attack
{
    public ThunderFang()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 422;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 65;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Thunder Fang");
        Description = "The user bites with electrified fangs. It may also make the target flinch or leave it with paralysis.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = true;

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
        isJawMove = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanParalyze;
        aiField3 = AIField.CanFlinch;

        effectChances.Add(10);
        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.MovesFirst(own) == true)
        {
            if (Core.Random.Next(0, 100) < GetEffectChance(1, own, battleScreen))
            {
                battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:thunderfang");
            }
            else
            {
                int chance = GetEffectChance(0, own, battleScreen);
                if (Core.Random.Next(0, 100) < chance)
                {
                    battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:thunderfang");
                }
            }
        }
        else
        {
            int chance = GetEffectChance(0, own, battleScreen);
            if (Core.Random.Next(0, 100) < chance)
            {
                battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:thunderfang");
            }
        }
    }

}
