using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Transform : Attack
{
    public Transform()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 144;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Transform");
        Description = "The user transforms into a copy of the target right down to having the same move set.";
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        focusOpponentPokemon = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        // Changes: Type, stats (except HP), stat modifications, moveset, species, shiny, ability, additionalvalue
        // Set istransformed to true
        // fail if target is transformed
        // apply image to sprite after transform

        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;

        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        if (op.IsTransformed == false && p.IsTransformed == false)
        {
            // Save old stats:
            p.OriginalNumber = p.Number;
            p.OriginalType1 = new Element(p.Type1.Type);
            p.OriginalType2 = new Element(p.Type2.Type);
            p.OriginalStats = [p.Attack, p.Defense, p.SpAttack, p.SpDefense, p.Speed];
            p.OriginalShiny = p.IsShiny == true ? 1 : 0;
            p.OriginalMoves = new List<BattleSystem.Attack>();
            p.OriginalMoves.AddRange(p.Attacks.ToArray());


            // Apply new stats:
            p.Number = op.Number;

            p.Type1 = new Element(op.Type1.Type);
            p.Type2 = new Element(op.Type2.Type);

            p.Attack = op.Attack;
            p.Defense = op.Defense;
            p.SpAttack = op.SpAttack;
            p.SpDefense = op.SpDefense;
            p.Speed = op.Speed;

            p.StatAttack = op.StatAttack;
            p.StatDefense = op.StatDefense;
            p.StatSpAttack = op.StatSpAttack;
            p.StatSpDefense = op.StatSpDefense;
            p.StatSpeed = op.StatSpeed;

            p.IsShiny = op.IsShiny;

            p.Attacks.Clear();
            for (int i = 0; i <= op.Attacks.Count - 1; i++)
            {
                p.Attacks.Add(GetAttackByID(op.Attacks[i].ID));
                p.Attacks[i].CurrentPP = 5;
            }

            p.Ability = Ability.GetAbilityByID(op.Ability.ID);

            p.IsTransformed = true;

            // Apply new image to sprite:
            battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(own, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(p, true), 0, 1, -1, -1));
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into " + op.GetName + "!"));
        }
        else
        {
            // Fails
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
