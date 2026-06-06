using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class PerishSong : Attack
{
    public PerishSong()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 195;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Perish Song");
        Description = "Any Pokémon that hears this song faints in three turns, unless it switches out of battle.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.All;
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
        isSoundMove = true;

        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int userPerishSongCount = 0;
        if (own == true)
        {
            userPerishSongCount = battleScreen.FieldEffects.PerishSongCount.Self;
        }
        else
        {
            userPerishSongCount = battleScreen.FieldEffects.PerishSongCount.Opponent;
        }

        if (userPerishSongCount == 0)
        {
            battleScreen.FieldEffects.PerishSongCount.Self = 4;
            battleScreen.FieldEffects.PerishSongCount.Opponent = 4;
            battleScreen.BattleQuery.Add(new TextQueryObject("All Pokémon hearing the song will faint in three turns!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
