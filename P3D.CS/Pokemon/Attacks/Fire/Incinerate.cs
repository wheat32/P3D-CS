using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class Incinerate : Attack
{
    public Incinerate()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 510;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 60;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Incinerate");
        Description = "The user attacks opposing Pokémon with fire. If a Pokémon is holding a certain item, such as a Berry, the item becomes burned up and unusable.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = false;

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
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        if (op.Item != null)
        {
            if (op.Item.IsBerry == true || op.Item.OriginalName.ToLower().EndsWith(" gem"))
            {
                String ItemID = String.Empty;
                if (op.Item.IsGameModeItem == true)
                {
                    ItemID = op.Item.gmID;
                }
                else
                {
                    ItemID = op.Item.ID.ToString();
                }
                battleScreen.Battle.RemoveHeldItem(own == false, own, battleScreen, op.GetDisplayName() + "'s " + op.Item.OneLineName() + " got burned up!", "move:incinerate");
            }
        }
    }

}
