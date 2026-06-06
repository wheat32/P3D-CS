using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Conversion2 : Attack
{
    public Conversion2()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 176;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Conversion 2");
        Description = "The user changes its type to make itself resistant to the type of the attack the opponent used last.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
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

        Attack lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Self;
        }

        // Conversion 2 will fail if the last move the target used was Struggle or if there is no type that resists that move.
        if (lastMove == null || lastMove.Name.ToLower == "struggle")
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
        else
        {
            List<Element> AllTypes = [];
            List<Element> ConsideredTypes = [];

            for (int i = 0; i <= 18; i++)
            {
                AllTypes.Add(new Element(i));
            }

            // Conversion 2 will not change the user to its current type. So Remove that combination.
            AllTypes.Remove(p.Type1);
            if (p.Type2 != null)
            {
                AllTypes.Remove(p.Type2);
            }

            // Remaining is the ones that Conversion 2 can change into. Now calculate which type can be considered.
            foreach (Element NewType in AllTypes)
            {
                if (BattleCalculation.ReverseTypeEffectiveness(Element.GetElementMultiplier(lastMove.Type, NewType)) < 1)
                {
                    ConsideredTypes.Add(NewType);
                }
            }

            if (ConsideredTypes.Count > 0)
            {
                Element SelectedType = ConsideredTypes(Core.Random.Next(0, ConsideredTypes.Count - 1));

                p.OriginalType1 = p.Type1;
                p.OriginalType2 = p.Type2;

                p.Type1 = SelectedType;
                p.Type2 = new Element(Element.Types.Blank);

                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into the " + SelectedType.ToString() + " type!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
