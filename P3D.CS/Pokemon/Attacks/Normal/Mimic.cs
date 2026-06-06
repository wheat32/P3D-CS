using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Mimic : Attack
{
    public Mimic()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 102;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Mimic");
        Description = "The user copies the target's last move. The move can be used during battle until the Pokémon is switched out.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
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
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        Attack moveToCopy = BattleSystem.Attack.GetAttackByID(op.Attacks.Last().ID);

        int[] failsMoves = {165, 166, 118, 448};

        if (p.KnowsMove(moveToCopy) == false && failsMoves.Contains(moveToCopy.ID) == false)
        {
            p.OriginalMoves = new List<BattleSystem.Attack>();
            p.OriginalMoves.AddRange(p.Attacks.ToArray());

            foreach (Attack m in p.Attacks)
            {
                if (m.ID == ID)
                {
                    p.Attacks.Remove(m);
                    break;
                }
            }

            p.Attacks.Add(moveToCopy);

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " learned " + moveToCopy.Name + "!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
