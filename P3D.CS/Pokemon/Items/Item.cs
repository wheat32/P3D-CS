using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Items;

public abstract class Item
{
    protected String _textureSource = @"Items\ItemSheet";
    protected Rectangle _textureRectangle;
    private Texture2D? _texture;

    // GameMode item fields
    public String gmID = String.Empty;
    public String gmName = String.Empty;
    public String gmPluralName = String.Empty;
    public String gmDescription = String.Empty;
    public String gmTextureSource = @"Items\GameModeItems";
    public Rectangle gmTextureRectangle;
    public bool gmIsBerry = false;
    public bool gmIsMail = false;
    public bool gmIsMegaStone = false;
    public bool gmIsPlate = false;
    public bool gmBattleSelectPokemon = true;
    public bool gmReplacesBattleQuery = false;

    public bool IsGameModeItem { get; set; } = false;

    public String TextureSource
    {
        get
        {
            return $"{_textureSource},{_textureRectangle.X},{_textureRectangle.Y},{_textureRectangle.Width},{_textureRectangle.Height}";
        }
    }

    public String GetDescription()
    {
        if (IsGameModeItem == true)
        {
            if (Localization.TokenExists($"item_desc_{gmID}") == true)
            {
                return Localization.GetString($"item_desc_{gmID}");
            }
            else
            {
                return gmDescription;
            }
        }
        else
        {
            if (Localization.TokenExists($"item_desc_{GetAttribute().Id}") == true)
            {
                return Localization.GetString($"item_desc_{GetAttribute().Id}");
            }
            else
            {
                return Description;
            }
        }
    }

    private ItemAttribute? _attribute;

    private ItemAttribute GetAttribute()
    {
        if (_attribute == null)
        {
            _attribute = (ItemAttribute)GetType().GetCustomAttributes(typeof(ItemAttribute), false)[0];
        }
        return _attribute;
    }

    public String OriginalName
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return gmName;
            }
            return GetAttribute().Name;
        }
    }

    public virtual String Name
    {
        get
        {
            if (IsGameModeItem == true)
            {
                if (Localization.TokenExists($"item_name_{gmID}") == true)
                {
                    return Localization.GetString($"item_name_{gmID}");
                }
                return gmName;
            }
            if (Localization.TokenExists($"item_name_{GetAttribute().Id}") == true)
            {
                return Localization.GetString($"item_name_{GetAttribute().Id}");
            }
            return GetAttribute().Name;
        }
    }

    public String OneLineName() => Name.Replace("~", "");

    public String OneLinePluralName()
    {
        if (IsGameModeItem == true)
        {
            if (Localization.TokenExists($"item_pluralname_{gmID}") == true)
            {
                return Localization.GetString($"item_pluralname_{gmID}").Replace("~", " ");
            }
            return gmPluralName.Replace("~", " ");
        }
        if (Localization.TokenExists($"item_pluralname_{ID}") == true)
        {
            return Localization.GetString($"item_pluralname_{ID}").Replace("~", " ");
        }
        return PluralName.Replace("~", " ");
    }

    public virtual int ID
    {
        get { return GetAttribute().Id; }
    }

    public virtual String PluralName => Name + "s";

    public virtual int PokeDollarPrice { get; protected set; } = 0;
    public virtual int BattlePointsPrice { get; } = 1;
    public virtual ItemTypes ItemType { get; } = ItemTypes.Standard;
    public virtual float CatchMultiplier { get; } = 1.0f;
    public virtual int MaxStack { get; } = 999;
    public virtual int SortValue { get; protected set; } = 0;

    public Texture2D? Texture
    {
        get
        {
            if (IsGameModeItem == true)
            {
                if (_texture == null)
                {
                    _texture = TextureManager.GetTexture(gmTextureSource, gmTextureRectangle, "");
                }
            }
            else
            {
                if (_texture == null)
                {
                    _texture = TextureManager.GetTexture(_textureSource, _textureRectangle, "");
                }
            }
            return _texture;
        }
    }

    public virtual String Description { get; protected set; } = String.Empty;
    public String AdditionalData { get; set; } = String.Empty;
    public virtual int FlingDamage { get; } = 30;
    public virtual bool CanBeTraded { get; protected set; } = true;
    public virtual bool CanBeHeld { get; } = true;
    public virtual bool CanBeUsed { get; } = true;
    public virtual bool CanBeUsedInBattle { get; } = true;
    public virtual bool CanBeTossed { get; protected set; } = true;

    public virtual bool BattleSelectPokemon
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return gmBattleSelectPokemon;
            }
            return true;
        }
    }

    public virtual bool ReplacesBattleQuery
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return gmReplacesBattleQuery;
            }
            return false;
        }
    }

    public virtual bool IsHealingItem { get; } = false;

    public virtual bool IsBall
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return ItemType == ItemTypes.Pokeballs;
            }
            return GetType().IsSubclassOf(typeof(Balls.BallItem));
        }
    }

    public bool IsBerry
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return gmIsBerry;
            }
            return GetType().IsSubclassOf(typeof(Berry));
        }
    }

    public bool IsMail
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return gmIsMail;
            }
            return GetType().IsSubclassOf(typeof(MailItem));
        }
    }

    public bool IsMegaStone
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return gmIsMegaStone;
            }
            return GetType().IsSubclassOf(typeof(MegaStone));
        }
    }

    public bool IsPlate
    {
        get
        {
            if (IsGameModeItem == true)
            {
                return gmIsPlate;
            }
            return GetType().IsSubclassOf(typeof(PlateItem));
        }
    }

    public static Color PlayerDialogueColor => new Color(0, 128, 227);

    public virtual void Use()
    {
        Logger.Debug("PLACEHOLDER FOR ITEM USE");
    }

    public void UseItemhandler(Object[] parameters)
    {
        if (UseOnPokemon((int)parameters[0]) == true)
        {
            Screen s = Core.CurrentScreen;
            while (s.PreScreen != null && s.Identification != Screen.Identifications.InventoryScreen)
            {
                s = s.PreScreen;
            }
            if (s.Identification == Screen.Identifications.InventoryScreen)
            {
                ((NewInventoryScreen)s).LoadItems();
            }
        }
    }

    public virtual bool UseOnPokemon(int pokeIndex)
    {
        if (pokeIndex < 0 || pokeIndex > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(pokeIndex), pokeIndex, "The index for a Pokémon in a player's party can only be between 0 and 5.");
        }
        Logger.Debug("PLACEHOLDER FOR ITEM USE ON POKEMON");
        return false;
    }

    public String RemoveItem()
    {
        String itemID = IsGameModeItem == true ? gmID : ID.ToString();
        Core.Player.Inventory.RemoveItem(itemID, 1);
        Screen s = Core.CurrentScreen;
        while (s.PreScreen != null && s.Identification != Screen.Identifications.InventoryScreen)
        {
            s = s.PreScreen;
        }
        if (s.Identification == Screen.Identifications.InventoryScreen)
        {
            ((NewInventoryScreen)s).LoadItems();
        }
        if (Core.Player.Inventory.GetItemAmount(itemID) <= 0)
        {
            return "*" + Localization.GetString("item_UsedLastItem", "There are no~[ITEMPLURALNAME] left.").Replace("[ITEMPLURALNAME]", OneLinePluralName());
        }
        return "";
    }

    private static Dictionary<ItemIdentifier, Type>? _itemBuffer;

    private static void LoadItemBuffer()
    {
        _itemBuffer = typeof(Item).Assembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(ItemAttribute), false).Length == 1)
            .ToDictionary(
                tt =>
                {
                    ItemAttribute attr = (ItemAttribute)tt.GetCustomAttributes(typeof(ItemAttribute), false)[0];
                    return new ItemIdentifier { Id = attr.Id, Name = attr.Name };
                },
                tt => tt);
    }

    public static Item? GetItemByID(String id)
    {
        if (id.Contains("gm") == true)
        {
            // TODO Phase 7: GameModeItemLoader.GetItemByID(id)
            return null;
        }

        if (_itemBuffer == null)
        {
            LoadItemBuffer();
        }

        if (String.IsNullOrEmpty(id) == false && int.TryParse(id, out int numericId) == true)
        {
            KeyValuePair<ItemIdentifier, Type> pair = _itemBuffer!.FirstOrDefault(p => p.Key.Id == numericId);
            if (pair.Value != null)
            {
                return (Item?)Activator.CreateInstance(pair.Value);
            }
        }
        return null;
    }

    public static Item? GetItemByName(String name)
    {
        if (_itemBuffer == null)
        {
            LoadItemBuffer();
        }

        KeyValuePair<ItemIdentifier, Type> pair = _itemBuffer!.FirstOrDefault(
            p => p.Key.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (pair.Value != null)
        {
            return (Item?)Activator.CreateInstance(pair.Value);
        }

        Logger.Log(Logger.LogTypes.Warning, $"Item.cs: Cannot find item with the name \"{name}\".");
        return null;
    }
}
