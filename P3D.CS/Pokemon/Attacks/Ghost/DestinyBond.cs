using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class DestinyBond : Attack
{
    public DestinyBond()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 194;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Destiny Bond");
        Description = "When this move is used, if the user faints, the Pokémon that landed the knockout hit also faints.";
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
        snatchAffected = false;
        mirrorMoveAffected = false;
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
            if (battleScreen.FieldEffects.DestinyBond.Self == false)
            {
                battleScreen.FieldEffects.DestinyBond.Self = true;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " is trying to take its foe down with it."));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.DestinyBond.Opponent == false)
            {
                battleScreen.FieldEffects.DestinyBond.Opponent = true;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " is trying to take its foe down with it."));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
