using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class AquaRing : Attack
{
    public AquaRing()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 392;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Aqua Ring");
        Description = "The user envelops itself in a veil made of water. It regains some HP on every turn.";
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
        if (own == true)
        {
            if (battleScreen.FieldEffects.AquaRing.Self == 0)
            {
                battleScreen.FieldEffects.AquaRing.Self = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " surrounded itself with a veil of water!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.AquaRing.Opponent == 0)
            {
                battleScreen.FieldEffects.AquaRing.Opponent = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " surrounded itself with a veil of water!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
