using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Growl : Attack
{
    public Growl()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 45;
        originalPP = 40;
        currentPP = 40;
        maxPP = 40;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Growl");
        Description = "The user growls in an endearing way, making the opposing team less wary. The foes' Attack stats are lowered.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
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
        isSoundMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.LowerAttack;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Attack", 1, "", "move:growl");
        if (b == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);

        MoveAnimation.AnimationPlaySound(currentPokemon.Number.ToString(), 0, 0, true);
        Entity SoundwaveEntity = MoveAnimation.SpawnEntity(new Vector3(0.25f, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Growl", new Rectangle(0, 0, 32, 32), ""), new Vector3(0.5F), 1, 0, 1);

        MoveAnimation.AnimationChangeTexture(SoundwaveEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Growl", new Rectangle(0, 32, 32, 32), ""), 1, 1);
        MoveAnimation.AnimationChangeTexture(SoundwaveEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Growl", new Rectangle(0, 0, 32, 32), ""), 2, 1);
        MoveAnimation.AnimationChangeTexture(SoundwaveEntity, true, TextureManager.GetTexture(@"Textures\Battle\Normal\Growl", new Rectangle(0, 32, 32, 32), ""), 3, 1);

        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
