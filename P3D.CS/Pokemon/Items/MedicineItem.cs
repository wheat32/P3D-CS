using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items;

public abstract class MedicineItem : Item
{
    public override ItemTypes ItemType { get; } = ItemTypes.Medicine;

    public bool HealPokemon(int pokeIndex, int hp)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];
        if (hp < 0)
        {
            hp = (int)(pokemon.MaxHP / (100 / (hp * -1)));
        }
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_IsFainted", "[POKEMONNAME]~is fainted!").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
            return false;
        }
        if (pokemon.HP == pokemon.MaxHP)
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_FullHP_Single", "[POKEMONNAME] has full~HP already.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
            return false;
        }
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
        {
            s = s.PreScreen;
        }
        if (s.Identification == Screen.Identifications.BattleScreen)
        {
            ((BattleSystem.BattleScreen)s).BattleMenu.Visible = false;
        }
        int diff = pokemon.MaxHP - pokemon.HP;
        diff = (int)MathHelper.Clamp(diff, 1, hp);
        pokemon.Heal(hp);
        Screen.TextBox.reDelay = 0.0f;
        String t = Localization.GetString("item_use_HealItem_Own", "Restored [POKEMONNAME]'s~HP by [HPAMOUNT].")
            .Replace("[POKEMONNAME]", pokemon.GetDisplayName())
            .Replace("[HPAMOUNT]", diff.ToString());
        t += RemoveItem();
        SoundManager.PlaySound("Use_Item", false);
        Screen.TextBox.Show(t, []);
        PlayerStatistics.Track("[17]Medicine Items used", 1);
        return true;
    }

    public bool CurePoison(int pokeIndex)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_IsFainted", "[POKEMONNAME]~is fainted!").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
            return false;
        }
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Poison || pokemon.Status == P3D.Pokemon.StatusProblems.BadPoison)
        {
            Screen s = Core.CurrentScreen;
            while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            {
                s = s.PreScreen;
            }
            if (s.Identification == Screen.Identifications.BattleScreen)
            {
                ((BattleSystem.BattleScreen)s).BattleMenu.Visible = false;
            }
            pokemon.Status = P3D.Pokemon.StatusProblems.None;
            Screen.TextBox.reDelay = 0.0f;
            String t = Localization.GetString("item_use_CurePoison_Single", "Cured the poison~of [POKEMONNAME].").Replace("[POKEMONNAME]", pokemon.GetDisplayName());
            t += RemoveItem();
            PlayerStatistics.Track("[17]Medicine Items used", 1);
            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(t, []);
            return true;
        }
        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(Localization.GetString("item_cannot_use_NotPoisoned_Single", "[POKEMONNAME] is not~poisoned.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
        return false;
    }

    public bool WakeUp(int pokeIndex)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_IsFainted", "[POKEMONNAME]~is fainted!").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
            return false;
        }
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Sleep)
        {
            Screen s = Core.CurrentScreen;
            while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            {
                s = s.PreScreen;
            }
            if (s.Identification == Screen.Identifications.BattleScreen)
            {
                ((BattleSystem.BattleScreen)s).BattleMenu.Visible = false;
            }
            pokemon.Status = P3D.Pokemon.StatusProblems.None;
            Screen.TextBox.reDelay = 0.0f;
            String t = Localization.GetString("item_use_CureSleep_Single", "Cured the sleep~of [POKEMONNAME].").Replace("[POKEMONNAME]", pokemon.GetDisplayName());
            t += RemoveItem();
            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(t, []);
            PlayerStatistics.Track("[17]Medicine Items used", 1);
            return true;
        }
        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(Localization.GetString("item_cannot_use_NotAsleep_Single", "[POKEMONNAME] is not~asleep.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
        return false;
    }

    public bool HealBurn(int pokeIndex)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_IsFainted", "[POKEMONNAME]~is fainted!").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
            return false;
        }
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Burn)
        {
            Screen s = Core.CurrentScreen;
            while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            {
                s = s.PreScreen;
            }
            if (s.Identification == Screen.Identifications.BattleScreen)
            {
                ((BattleSystem.BattleScreen)s).BattleMenu.Visible = false;
            }
            pokemon.Status = P3D.Pokemon.StatusProblems.None;
            Screen.TextBox.reDelay = 0.0f;
            String t = Localization.GetString("item_use_CureBurn_Single", "Cured the burn~of [POKEMONNAME].").Replace("[POKEMONNAME]", pokemon.GetDisplayName());
            t += RemoveItem();
            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(t, []);
            PlayerStatistics.Track("[17]Medicine Items used", 1);
            return true;
        }
        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(Localization.GetString("item_cannot_use_NotBurned_Single", "[POKEMONNAME] is not~burned.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
        return false;
    }

    public bool HealIce(int pokeIndex)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_IsFainted", "[POKEMONNAME]~is fainted!").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
            return false;
        }
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Freeze)
        {
            Screen s = Core.CurrentScreen;
            while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            {
                s = s.PreScreen;
            }
            if (s.Identification == Screen.Identifications.BattleScreen)
            {
                ((BattleSystem.BattleScreen)s).BattleMenu.Visible = false;
            }
            pokemon.Status = P3D.Pokemon.StatusProblems.None;
            Screen.TextBox.reDelay = 0.0f;
            String t = Localization.GetString("item_use_CureIce_Single", "Cured the ice~of [POKEMONNAME].").Replace("[POKEMONNAME]", pokemon.GetDisplayName());
            t += RemoveItem();
            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(t, []);
            PlayerStatistics.Track("[17]Medicine Items used", 1);
            return true;
        }
        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(Localization.GetString("item_cannot_use_NotFrozen_Single", "[POKEMONNAME] is not~frozen.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
        return false;
    }

    public bool HealParalyze(int pokeIndex)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Fainted)
        {
            Screen.TextBox.reDelay = 0.0f;
            Screen.TextBox.Show(Localization.GetString("item_cannot_use_IsFainted", "[POKEMONNAME]~is fainted!").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
            return false;
        }
        if (pokemon.Status == P3D.Pokemon.StatusProblems.Paralyzed)
        {
            Screen s = Core.CurrentScreen;
            while (s.Identification != Screen.Identifications.BattleScreen && s.PreScreen != null)
            {
                s = s.PreScreen;
            }
            if (s.Identification == Screen.Identifications.BattleScreen)
            {
                ((BattleSystem.BattleScreen)s).BattleMenu.Visible = false;
            }
            pokemon.Status = P3D.Pokemon.StatusProblems.None;
            Screen.TextBox.reDelay = 0.0f;
            String t = Localization.GetString("item_use_CureParalysis_Single", "Cured the paralysis~of [POKEMONNAME].").Replace("[POKEMONNAME]", pokemon.GetDisplayName());
            t += RemoveItem();
            SoundManager.PlaySound("Use_Item", false);
            Screen.TextBox.Show(t, []);
            PlayerStatistics.Track("[17]Medicine Items used", 1);
            return true;
        }
        Screen.TextBox.reDelay = 0.0f;
        Screen.TextBox.Show(Localization.GetString("item_cannot_use_NotParalyzed_Single", "[POKEMONNAME] is not~paralyzed.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()), []);
        return false;
    }
}
