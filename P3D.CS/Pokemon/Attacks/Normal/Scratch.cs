using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Scratch : Attack
{
    public Scratch()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 10;
        originalPP = 35;
        currentPP = 35;
        maxPP = 35;
        Power = 40;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Scratch");
        Description = "Hard, pointed, and sharp claws rake the target to inflict damage.";
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

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Scratch", 0.5f, 2.5f);
        int TextureXOffset = 0;
        if (battleFlip == true)
        {
            TextureXOffset = 32;
        }
        Entity ScratchEntity = MoveAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Scratch", new Rectangle(TextureXOffset, 0, 32, 32), ""), new Vector3(0.5F), 1, 0, 0.5f);
        MoveAnimation.AnimationChangeTexture(ScratchEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Scratch", new Rectangle(TextureXOffset, 32, 32, 32), ""), 0.5f, 0.5f);
        MoveAnimation.AnimationChangeTexture(ScratchEntity, true, TextureManager.GetTexture(@"Textures\Battle\Normal\Scratch", new Rectangle(TextureXOffset, 64, 32, 32), ""), 1, 0.5f);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
