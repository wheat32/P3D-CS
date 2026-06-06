using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Bug;

public class StickyWeb : Attack
{
    public StickyWeb()
    {
        // #Definitions
        type = new Element(Element.Types.Bug);
        ID = 564;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Sticky Web");
        Description = "The user weaves a sticky net around the opposing team, which lowers their Speed stat upon switching into battle.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = true;
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


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int stickyweb = 0;
        if (own == true)
        {
            stickyweb = battleScreen.FieldEffects.StickyWeb.Self;
        }
        else
        {
            stickyweb = battleScreen.FieldEffects.StickyWeb.Opponent;
        }
        if (stickyweb < 1)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.StickyWeb.Self += 1;
                battleScreen.BattleQuery.Add(new TextQueryObject("A sticky web has been laid beneath the opposite team's feet!"));
            }
            else
            {
                battleScreen.FieldEffects.StickyWeb.Opponent += 1;
                battleScreen.BattleQuery.Add(new TextQueryObject("A sticky web has been laid beneath your team's feet!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
