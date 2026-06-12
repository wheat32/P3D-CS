using Microsoft.Xna.Framework;

namespace P3D.Items;

public abstract class TechMachine : Item
{
    public BattleSystem.Attack Attack { get; }
    public bool IsTM { get; private set; } = true;
    public int TechID { get; private set; } = 0;
    public int HiddenID { get; private set; } = 0;

    public bool CanTeachAlways = false;
    public bool CanTeachWhenFullyEvolved = false;
    public bool CanTeachWhenGender = false;

    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeHeld { get; } = false;
    public override bool CanBeTraded { get; protected set; } = true;
    public override bool CanBeTossed { get; protected set; } = true;
    public override int SortValue { get; protected set; }
    public override String Description { get; protected set; } = String.Empty;
    public override ItemTypes ItemType { get; } = ItemTypes.Machines;
    public override int PokeDollarPrice { get; protected set; }

    protected TechMachine(bool isTM, int price, int attackID)
    {
        Attack = BattleSystem.Attack.GetAttackByID(attackID);
        IsTM = isTM;
        TechID = ID - 190;
        HiddenID = ID - 242;
        PokeDollarPrice = price;

        if (IsTM == false)
        {
            CanBeTraded = false;
            CanBeTossed = false;
            SortValue = -100000 + int.Parse(Name[3..]);
        }
        else
        {
            SortValue = ID;
        }

        Description = $"Teaches \"{Attack.Name}\" to a Pokémon.";
        SetTextureRectangle();
    }

    private void SetTextureRectangle()
    {
        Rectangle r = new Rectangle(144, 168, 24, 24);
        switch (Attack.type.Type)
        {
            case Element.Types.Blank:
            case Element.Types.Normal:
                r = new Rectangle(144, 168, 24, 24);
                break;
            case Element.Types.Bug:
                r = new Rectangle(24, 192, 24, 24);
                break;
            case Element.Types.Dark:
                r = new Rectangle(384, 168, 24, 24);
                break;
            case Element.Types.Dragon:
                r = new Rectangle(408, 168, 24, 24);
                break;
            case Element.Types.Electric:
                r = new Rectangle(288, 168, 24, 24);
                break;
            case Element.Types.Fairy:
                r = new Rectangle(72, 264, 24, 24);
                break;
            case Element.Types.Fighting:
                r = new Rectangle(168, 168, 24, 24);
                break;
            case Element.Types.Fire:
                r = new Rectangle(360, 168, 24, 24);
                break;
            case Element.Types.Flying:
                r = new Rectangle(0, 192, 24, 24);
                break;
            case Element.Types.Ghost:
                r = new Rectangle(480, 168, 24, 24);
                break;
            case Element.Types.Grass:
                r = new Rectangle(336, 168, 24, 24);
                break;
            case Element.Types.Ground:
                r = new Rectangle(456, 168, 24, 24);
                break;
            case Element.Types.Ice:
                r = new Rectangle(312, 168, 24, 24);
                break;
            case Element.Types.Poison:
                r = new Rectangle(264, 168, 24, 24);
                break;
            case Element.Types.Psychic:
                r = new Rectangle(216, 168, 24, 24);
                break;
            case Element.Types.Rock:
                r = new Rectangle(240, 168, 24, 24);
                break;
            case Element.Types.Steel:
                r = new Rectangle(432, 168, 24, 24);
                break;
            case Element.Types.Water:
                r = new Rectangle(192, 168, 24, 24);
                break;
        }
        _textureRectangle = r;
    }

    public override void Use()
    {
        if (Core.Player.Pokemons.Count > 0)
        {
            SoundManager.PlaySound(@"PC\LogOn", false);
            PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, this, UseOnPokemon,
                Localization.GetString("global_Learn", "Learn") + " " + Attack.Name, true)
            {
                Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection,
                CanExit = true,
            };
            selScreen.SelectedObject += UseItemhandler;
            Core.SetScreen(selScreen);
            ((PartyScreen)Core.CurrentScreen).SetupLearnAttack(Attack, 1, this);
        }
        else
        {
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_NoPokemon", "You don't have any Pokémon."), [], false, false);
        }
    }

    public override bool UseOnPokemon(int pokeIndex)
    {
        Pokemon p = Core.Player.Pokemons[pokeIndex];
        String reason = CanTeach(p);
        if (reason.Equals("") == true)
        {
            if (p.Attacks.Count == 4)
            {
                Core.SetScreen(new LearnAttackScreen(Core.CurrentScreen, p, Attack, ID.ToString()));
                return true;
            }
            else
            {
                String lastItemText = String.Empty;
                if (IsTM == true && bool.Parse(GameModeManager.GetGameRuleValue("SingleUseTM", "0")) == true)
                {
                    lastItemText = "*" + RemoveItem();
                }
                p.Attacks.Add(BattleSystem.Attack.GetAttackByID(Attack.ID));
                SoundManager.PlaySound("success_small", false);
                Screen.TextBox.Show(
                    Localization.GetString("learn_move_PokemonLearnedMove", "[POKEMONNAME] learned~[MOVENAME]!")
                        .Replace("[POKEMONNAME]", p.GetDisplayName())
                        .Replace("[MOVENAME]", Attack.Name) + lastItemText, [], false, false);
                PlayerStatistics.Track("TMs/HMs used", 1);
                return true;
            }
        }
        Screen.TextBox.Show(reason, [], false, false);
        return false;
    }

    public String CanTeach(Pokemon p)
    {
        if (p.IsEgg == true)
        {
            return Localization.GetString("learn_move_EggCannotLearn", "Egg cannot learn~[MOVENAME]!").Replace("[MOVENAME]", Attack.Name);
        }
        foreach (BattleSystem.Attack knownAttack in p.Attacks)
        {
            if (knownAttack.ID == Attack.ID)
            {
                return Localization.GetString("learn_move_AlreadyKnowsTheMove", "[POKEMONNAME] already~knows [MOVENAME].").Replace("[POKEMONNAME]", p.GetDisplayName()).Replace("[MOVENAME]", Attack.Name);
            }
        }
        if (p.machines.Contains(Attack.ID) == true)
        {
            return "";
        }
        foreach (List<BattleSystem.Attack> aList in p.attackLearns.Values)
        {
            foreach (BattleSystem.Attack learnAttack in aList)
            {
                if (learnAttack.ID == Attack.ID)
                {
                    return "";
                }
            }
        }
        if (CanTeachAlways == true && p.machines.Count > 0)
        {
            return "";
        }
        if (CanTeachWhenFullyEvolved == true && p.IsFullyEvolved() == true && p.machines.Count > 0)
        {
            return "";
        }
        if (CanTeachWhenGender == true && p.Gender != Pokemon.Genders.Genderless && p.machines.Count > 0)
        {
            return "";
        }
        if (p.CanLearnAllMachines == true)
        {
            return "";
        }
        return Localization.GetString("learn_move_PokemonCannotLearn", "[POKEMONNAME] cannot learn~[MOVENAME]!").Replace("[POKEMONNAME]", p.GetDisplayName()).Replace("[MOVENAME]", Attack.Name);
    }
}
