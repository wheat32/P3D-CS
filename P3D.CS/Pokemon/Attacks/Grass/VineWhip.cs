using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class VineWhip : Attack
{
    public VineWhip()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 22;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 45;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Vine Whip");
        Description = "The target is struck with slender, whiplike vines to inflict damage.";
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

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Grass\VineWhip_Start", 0.5f, 1.25f);
        MoveAnimation.AnimationOscillateMove(null, false, new Vector3(0, 0, -0.15f), 0.035f, false, 0, 0, 0, 0, new Vector3(0, 0, 1));
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Grass\VineWhip_Hit", 0, 2.5f);
        int TextureXOffset = 0;
        if (battleFlip == true)
        {
            TextureXOffset = 32;
        }
        Entity VineWhipEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Grass\VineWhip", new Rectangle(TextureXOffset, 0, 32, 32), ""), new Vector3(0.5F), 1, 0, 0.5f);
        MoveAnimation.AnimationChangeTexture(VineWhipEntity, false, TextureManager.GetTexture(@"Textures\Battle\Grass\VineWhip", new Rectangle(TextureXOffset, 32, 32, 32), ""), 0.5f, 0.5f);
        MoveAnimation.AnimationChangeTexture(VineWhipEntity, false, TextureManager.GetTexture(@"Textures\Battle\Grass\VineWhip", new Rectangle(TextureXOffset, 64, 32, 32), ""), 1, 0.5f);
        MoveAnimation.AnimationChangeTexture(VineWhipEntity, true, TextureManager.GetTexture(@"Textures\Battle\Grass\VineWhip", new Rectangle(TextureXOffset, 96, 32, 32), ""), 1.5f, 0.5f);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
