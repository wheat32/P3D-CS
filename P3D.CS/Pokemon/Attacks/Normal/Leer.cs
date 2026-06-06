using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Leer : Attack
{
    public Leer()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 43;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Leer");
        Description = "The user gains an intimidating leer with sharp eyes. The opposing team’s Defense stats are reduced.";
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        canHitSleeping = false;
        // #End

        aiField1 = AIField.LowerDefense;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Defense", 1, "", "move:leer");
        if (b == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

    public override void InternalUserPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Leer", 0, 0);
        Object SpawnEntity = MoveAnimation.SpawnEntity(new Vector3(0, 0.1f, 0.1f), TextureManager.GetTexture(@"Textures\Battle\Normal\Leer"), new Vector3(0.5F), 1.0F, 0, 2);
        MoveAnimation.AnimationScale(SpawnEntity, false, 0.7, 0.7, 0.7, 0.05, 0, 0.5);
        MoveAnimation.AnimationScale(SpawnEntity, false, 0.5, 0.5, 0.5, 0.05, 0.5, 0.5);
        MoveAnimation.AnimationScale(SpawnEntity, false, 0.7, 0.7, 0.7, 0.05, 1.0, 0.5);
        MoveAnimation.AnimationScale(SpawnEntity, false, 0.5, 0.5, 0.5, 0.05, 1.5, 0.5);
        MoveAnimation.AnimationFade(SpawnEntity, true, 1.0F, 0.0F, 2, 0);
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

    public override void InternalOpponentPokemonMoveAnimation(BattleScreen battleScreen, bool battleFlip, Pokemon currentPokemon, NPC currentEntity)
    {
        AnimationQueryObject MoveAnimation = new AnimationQueryObject(currentEntity, battleFlip);
        MoveAnimation.AnimationOscillateMove(null, false, new Vector3(0, 0, 0.05f), 0.035, true, 3, 0, 0.5, 0, new Vector3(0, 0, 1));
        battleScreen.BattleQuery.Add(MoveAnimation);
    }

}
