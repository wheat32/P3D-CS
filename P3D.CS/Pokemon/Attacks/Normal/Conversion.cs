using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Conversion : Attack
{
    public Conversion()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 160;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Conversion");
        Description = "The user changes its type to become the same type as the move at the top of the list of moves it knows.";
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
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        Element newType = GameModeElementLoader.GetElementByID(p.Attacks[0].Type.Type);

        if (p.Type1.Type != newType.Type || p.Type2.Type != Element.Types.Blank)
        {
            p.OriginalType1 = GameModeElementLoader.GetElementByID(p.Type1.Type);
            p.OriginalType2 = GameModeElementLoader.GetElementByID(p.Type2.Type);

            p.Type1 = newType;
            p.Type2.Type = Element.Types.Blank;

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into the " + newType.ToString() + " type!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
