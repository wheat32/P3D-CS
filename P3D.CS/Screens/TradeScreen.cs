using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class TradeScreen : Screen
{
    private enum MenuStates
    {
        MainPage,
        BuyItems,
        SellItems,
        BuyItemsCategory,
        SellItemsCategory,
        SellItemsConfirmation
    }

    private MenuStates _menuState = MenuStates.MainPage;
    private Items.ItemTypes _currentCategory = Items.ItemTypes.Medicine;

    private int _mainCursor = 0;
    private int _categoryCursor = 0;
    private int _buySellCursor = 0;
    private int _buySellScroll = 0;
    private int _categoryScroll = 0;

    private int Scroll
    {
        get
        {
            switch (_menuState)
            {
                case MenuStates.MainPage:
                    return 0;
                case MenuStates.BuyItems:
                case MenuStates.SellItems:
                case MenuStates.SellItemsConfirmation:
                    return _buySellScroll;
                case MenuStates.BuyItemsCategory:
                case MenuStates.SellItemsCategory:
                    return _categoryScroll;
            }
            return 0;
        }
        set
        {
            switch (_menuState)
            {
                case MenuStates.BuyItems:
                case MenuStates.SellItems:
                case MenuStates.SellItemsConfirmation:
                    _buySellScroll = value;
                    break;
                case MenuStates.BuyItemsCategory:
                case MenuStates.SellItemsCategory:
                    _categoryScroll = value;
                    break;
            }
        }
    }

    private int Cursor
    {
        get
        {
            switch (_menuState)
            {
                case MenuStates.MainPage:
                    return _mainCursor;
                case MenuStates.BuyItems:
                case MenuStates.SellItems:
                case MenuStates.SellItemsConfirmation:
                    return _buySellCursor;
                case MenuStates.BuyItemsCategory:
                case MenuStates.SellItemsCategory:
                    return _categoryCursor;
            }
            return 0;
        }
        set
        {
            switch (_menuState)
            {
                case MenuStates.MainPage:
                    _mainCursor = value;
                    break;
                case MenuStates.BuyItems:
                case MenuStates.SellItems:
                case MenuStates.SellItemsConfirmation:
                    _buySellCursor = value;
                    break;
                case MenuStates.BuyItemsCategory:
                case MenuStates.SellItemsCategory:
                    _categoryCursor = value;
                    break;
            }
        }
    }

    public enum Currencies
    {
        Pokédollar,
        BattlePoints,
        Coins
    }

    public struct TradeItem
    {
        public TradeItem(String itemID, int amount, int price, Currencies currency)
        {
            ItemID = itemID;
            Amount = amount;

            if (price == -1)
            {
                Items.Item item = GetItem();
                if (currency == Currencies.BattlePoints)
                    Price = item.BattlePointsPrice;
                else if (currency == Currencies.Pokédollar)
                    Price = item.PokeDollarPrice;
                else
                    Price = 0;
            }
            else
            {
                Price = price;
            }
        }

        public String ItemID;
        public int Price;
        public int Amount;

        public Items.Item GetItem() => Items.Item.GetItemByID(ItemID);

        public int SellPrice() => (int)Math.Ceiling(Price / 2.0);
    }

    private List<TradeItem> _tradeItems = [];
    private List<TradeItem> _possibleStoreItems = [];
    private bool _canBuyItems = true;
    private bool _canSellItems = true;
    private Currencies _currency = Currencies.Pokédollar;
    private String _shopIdentifier = String.Empty;

    private Texture2D _texture = null!;
    private int _tileOffset = 0;
    private String _title = String.Empty;

    public TradeScreen(Screen currentScreen, String storeString, bool canBuy, bool canSell,
                       String currencyIndicator, String shopIdentifier)
    {
        PreScreen = currentScreen;

        String[] itemArr = storeString.Split(['}'], StringSplitOptions.RemoveEmptyEntries);

        SetCurrency(currencyIndicator);

        foreach (String lItemRaw in itemArr)
        {
            String lItem = lItemRaw.Remove(0, 1);
            String[] itemData = lItem.Split('|');

            if (shopIdentifier != String.Empty)
            {
                _shopIdentifier = shopIdentifier;
                if (ActionScript.IsRegistered(_shopIdentifier + "_" + itemData[0]) == true)
                {
                    if (ScriptConversion.ToInteger(itemData[1]) != -1)
                    {
                        Object[] registerContent = ActionScript.GetRegisterValue(shopIdentifier + "_" + itemData[0]);
                        if ((int)registerContent[0] < ScriptConversion.ToInteger(itemData[1]))
                        {
                            int resultAmount = ScriptConversion.ToInteger(itemData[1]);
                            if ((int)registerContent[0] > 0)
                                resultAmount = ScriptConversion.ToInteger(itemData[1]) - (int)registerContent[0];
                            _tradeItems.Add(new TradeItem(itemData[0], resultAmount, ScriptConversion.ToInteger(itemData[2]), _currency));
                            _possibleStoreItems.Add(new TradeItem(itemData[0], -1, ScriptConversion.ToInteger(itemData[2]), _currency));
                        }
                    }
                }
                else
                {
                    _tradeItems.Add(new TradeItem(itemData[0], ScriptConversion.ToInteger(itemData[1]), ScriptConversion.ToInteger(itemData[2]), _currency));
                    _possibleStoreItems.Add(new TradeItem(itemData[0], -1, ScriptConversion.ToInteger(itemData[2]), _currency));
                }
            }
            else
            {
                _tradeItems.Add(new TradeItem(itemData[0], ScriptConversion.ToInteger(itemData[1]), ScriptConversion.ToInteger(itemData[2]), _currency));
                _possibleStoreItems.Add(new TradeItem(itemData[0], -1, ScriptConversion.ToInteger(itemData[2]), _currency));
            }
        }

        if (_tradeItems.Count > 0)
            _tradeItems = _tradeItems.OrderBy(i => i.GetItem().OneLineName()).ToList();
        if (_possibleStoreItems.Count > 0)
            _possibleStoreItems = _possibleStoreItems.OrderBy(i => i.GetItem().OneLineName()).ToList();

        _texture = TextureManager.GetTexture("GUI\\Menus\\General");

        MouseVisible = true;
        CanMuteAudio = true;
        CanBePaused = true;

        _canBuyItems = canBuy;
        _canSellItems = canSell;

        _title = Localization.GetString("shop_screen_title_store", "Store");

        CreateMainMenuButtons();
    }

    private void CreateMainMenuButtons()
    {
        if (_mainMenuButtons.Count == 0)
        {
            if (_canBuyItems == true)
                _mainMenuButtons.Add(Localization.GetString("shop_screen_button_buy", "Buy"));
            if (_canSellItems == true)
                _mainMenuButtons.Add(Localization.GetString("shop_screen_button_sell", "Sell"));
            _mainMenuButtons.Add(Localization.GetString("shop_screen_button_exit", "Exit"));
        }
    }

    private void SetCurrency(String currencyIndicator)
    {
        switch (currencyIndicator.ToLower())
        {
            case "p":
            case "pokedollar":
            case "pokédollar":
            case "poke":
            case "poké":
            case "poke dollar":
            case "poké dollar":
            case "money":
                _currency = Currencies.Pokédollar;
                break;
            case "bp":
            case "battlepoints":
            case "battle points":
                _currency = Currencies.BattlePoints;
                break;
            case "c":
            case "coins":
                _currency = Currencies.Coins;
                break;
        }
    }

    public override void Update()
    {
        switch (_menuState)
        {
            case MenuStates.MainPage:
                UpdateMain();
                break;
            case MenuStates.BuyItemsCategory:
                UpdateBuyCategory();
                break;
            case MenuStates.BuyItems:
                UpdateBuyItems();
                break;
            case MenuStates.SellItemsCategory:
                UpdateSellCategory();
                break;
            case MenuStates.SellItems:
                UpdateSellItems();
                break;
            case MenuStates.SellItemsConfirmation:
                UpdateSellConfirmation();
                break;
        }

        _tileOffset += 1;
        if (_tileOffset >= 64)
            _tileOffset = 0;
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y + _tileOffset, 128, 64), new Rectangle(48, 0, 16, 16), Color.White);

        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 200), new Color(42, 167, 198), new Color(42, 167, 198, 0), false, -1);
        Canvas.DrawGradient(new Rectangle(0, Core.windowSize.Height - 200, Core.windowSize.Width, 200), new Color(42, 167, 198, 0), new Color(42, 167, 198), false, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, _title, new Vector2(100, 24), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

        switch (_menuState)
        {
            case MenuStates.MainPage:
                DrawMain();
                break;
            case MenuStates.BuyItemsCategory:
                DrawBuyCategory();
                break;
            case MenuStates.BuyItems:
                DrawBuyItems();
                break;
            case MenuStates.SellItemsCategory:
                DrawSellCategory();
                break;
            case MenuStates.SellItems:
                DrawSellItems();
                break;
            case MenuStates.SellItemsConfirmation:
                DrawSellConfirmation();
                break;
        }
    }

    #region Mainscreen

    private List<String> _mainMenuButtons = [];

    private void UpdateMain()
    {
        _title = Localization.GetString("shop_screen_title_store", "Store");

        if (Controls.Up(true, true, true, true, true, true) == true)
        {
            Cursor -= 1;
            if (Controls.ShiftDown() == true)
                Cursor -= 4;
        }
        if (Controls.Down(true, true, true, true, true, true) == true)
        {
            Cursor += 1;
            if (Controls.ShiftDown() == true)
                Cursor += 4;
        }
        Cursor = Cursor.Clamp(0, _mainMenuButtons.Count - 1);

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i < _mainMenuButtons.Count; i++)
            {
                if (new Rectangle(100, 100 + i * 96, 64 * 7, 64).Contains(MouseHandler.MousePosition) == true)
                {
                    if (i == Cursor)
                    {
                        SoundManager.PlaySound("select");
                        ClickMainButton();
                    }
                    else
                    {
                        Cursor = i;
                    }
                }
            }
        }

        if (Controls.Accept(false, true, true) == true)
        {
            SoundManager.PlaySound("select");
            ClickMainButton();
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            SoundManager.PlaySound("select");
            ButtonMainExit();
        }
    }

    private void ClickMainButton()
    {
        String selected = _mainMenuButtons[Cursor];
        if (selected == Localization.GetString("shop_screen_button_buy", "Buy"))
            ButtonMainBuy();
        else if (selected == Localization.GetString("shop_screen_button_sell", "Sell"))
            ButtonMainSell();
        else if (selected == Localization.GetString("shop_screen_button_exit", "Exit"))
            ButtonMainExit();
    }

    private void ButtonMainBuy()
    {
        _menuState = MenuStates.BuyItemsCategory;
        Cursor = 0;
        Scroll = 0;
        LoadBuyCategoriesItems();
    }

    private void ButtonMainSell()
    {
        _menuState = MenuStates.SellItemsCategory;
        Cursor = 0;
        Scroll = 0;
        LoadSellCategoryItems();
    }

    private void ButtonMainExit()
    {
        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, PreScreen, Color.White, false));
    }

    private void DrawMain()
    {
        int y = 100;
        foreach (String b in _mainMenuButtons)
        {
            DrawButton(new Vector2(100, y), 5, b, 16);
            y += 96;
        }
        DrawMainCursor();
    }

    private void DrawMainCursor()
    {
        Vector2 cPosition = new Vector2(380, 100 + Cursor * 96 - 42);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), Color.White);
    }

    #endregion

    #region BuyCategoryScreen

    private List<Items.ItemTypes> _loadedBuyCategories = [];

    private void LoadBuyCategoriesItems()
    {
        _loadedBuyCategories.Clear();
        foreach (TradeItem i in _tradeItems)
        {
            Items.Item item = i.GetItem();
            if (_loadedBuyCategories.Contains(item.ItemType) == false && item.CanBeTraded == true && (i.Amount == -1 || i.Amount > 0))
                _loadedBuyCategories.Add(item.ItemType);
        }
        _loadedBuyCategories = _loadedBuyCategories.OrderBy(c => (int)c).ToList();
    }

    private void UpdateBuyCategory()
    {
        _title = Localization.GetString("shop_screen_title_BuyItems", "Buy Items");

        if (_loadedBuyCategories.Count > 0)
        {
            if (Controls.Down(true, true, true, true, true, true) == true)
            {
                Cursor += 1;
                if (Controls.ShiftDown() == true)
                    Cursor += 4;
            }
            if (Controls.Up(true, true, true, true, true, true) == true)
            {
                Cursor -= 1;
                if (Controls.ShiftDown() == true)
                    Cursor -= 4;
            }

            while (Cursor > 5) { Cursor -= 1; Scroll += 1; }
            while (Cursor < 0) { Cursor += 1; Scroll -= 1; }

            if (_loadedBuyCategories.Count < 7)
                Scroll = 0;
            else
                Scroll = Scroll.Clamp(0, _loadedBuyCategories.Count - 6);

            if (_loadedBuyCategories.Count < 6)
                Cursor = Cursor.Clamp(0, _loadedBuyCategories.Count - 1);
            else
                Cursor = Cursor.Clamp(0, 5);

            if (Controls.Accept(true, false, false) == true)
            {
                for (int i = Scroll; i <= Scroll + 5; i++)
                {
                    if (i <= _loadedBuyCategories.Count - 1)
                    {
                        if (new Rectangle(100, 100 + (i - Scroll) * 96, 64 * 7, 64).Contains(MouseHandler.MousePosition) == true)
                        {
                            if (i == Scroll + Cursor)
                            {
                                SoundManager.PlaySound("select");
                                ButtonBuyCategoriesAccept();
                            }
                            else
                            {
                                Cursor = i - Scroll;
                            }
                        }
                    }
                }
            }

            if (Controls.Accept(false, true, true) == true)
            {
                SoundManager.PlaySound("select");
                ButtonBuyCategoriesAccept();
            }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            SoundManager.PlaySound("select");
            _menuState = MenuStates.MainPage;
        }
    }

    private void ButtonBuyCategoriesAccept()
    {
        _currentCategory = _loadedBuyCategories[Cursor + Scroll];
        _menuState = MenuStates.BuyItems;
        Cursor = 0;
        Scroll = 0;
        LoadBuyItemsList();
    }

    private void DrawBuyCategory()
    {
        if (_loadedBuyCategories.Count > 0)
        {
            for (int i = Scroll; i <= Scroll + 5; i++)
            {
                if (i <= _loadedBuyCategories.Count - 1)
                {
                    int p = i - Scroll;
                    DrawButton(new Vector2(100, 100 + p * 96), 5,
                        Localization.GetString("item_category_" + _loadedBuyCategories[i].ToString(), _loadedBuyCategories[i].ToString()),
                        16, GetItemTypeTexture(_loadedBuyCategories[i]));
                }
            }

            Canvas.DrawRectangle(new Rectangle(580, 100, 240, 48), new Color(255, 255, 255, 127));

            if (_loadedBuyCategories.Count > 0)
            {
                int x = 0;
                int y = 0;

                foreach (TradeItem i in _tradeItems)
                {
                    if (i.Amount != 0)
                    {
                        Items.Item item = i.GetItem();
                        if (item.ItemType == _loadedBuyCategories[Cursor + Scroll])
                        {
                            Core.SpriteBatch.Draw(item.Texture, new Rectangle(580 + x * 48, 100 + y * 48, 48, 48), Color.White);
                            x += 1;
                            if (x == 5)
                            {
                                x = 0;
                                y += 1;
                                Canvas.DrawRectangle(new Rectangle(580, 100 + y * 48, 240, 48), new Color(255, 255, 255, 127));
                            }
                        }
                    }
                }
            }

            DrawMainCursor();
        }
        else
        {
            DrawBanner(new Vector2((float)(Core.windowSize.Width / 2 - 250), (float)(Core.windowSize.Height / 2 - 50)), 100,
                Localization.GetString("shop_screen_buy_NoItemsToBuy", "There are no items to buy."), FontManager.MainFont, 500);
        }
    }

    #endregion

    #region BuyItemsScreen

    private float _buySellSparkleRotation = 0.0F;
    private float _buySellItemSize = 192.0F;
    private bool _buySellItemShrinking = true;

    private List<TradeItem> _buyItemsList = [];
    private int _buyItemsAmount = 1;
    private bool _buyItemsShowDescription = false;

    private void LoadBuyItemsList()
    {
        _buyItemsList.Clear();
        foreach (TradeItem i in _tradeItems)
        {
            Items.Item item = i.GetItem();
            if (item.ItemType == _currentCategory && i.Amount != 0)
                _buyItemsList.Add(i);
        }
        _buyItemsList = _buyItemsList.OrderBy(i => i.GetItem().OneLineName()).ToList();
    }

    private void UpdateBuyItems()
    {
        _title = Localization.GetString("shop_screen_title_BuyCategory", "Buy [CATEGORY]")
            .Replace("[CATEGORY]", Localization.GetString("item_category_" + _currentCategory.ToString(), _currentCategory.ToString()));

        if (Controls.Down(true, true, true, true, true, true) == true)
        {
            Cursor += 1;
            if (Controls.ShiftDown() == true)
                Cursor += 4;
        }
        if (Controls.Up(true, true, true, true, true, true) == true)
        {
            Cursor -= 1;
            if (Controls.ShiftDown() == true)
                Cursor -= 4;
        }
        if (Controls.Right(true, true, false, true, true, true) == true)
        {
            _buyItemsAmount += 1;
            if (Controls.ShiftDown() == true)
                _buyItemsAmount += 4;
        }
        if (Controls.Left(true, true, false, true, true, true) == true)
        {
            _buyItemsAmount -= 1;
            if (Controls.ShiftDown() == true)
                _buyItemsAmount -= 4;
        }

        while (Cursor > 5) { Cursor -= 1; Scroll += 1; }
        while (Cursor < 0) { Cursor += 1; Scroll -= 1; }

        if (_buyItemsList.Count < 7)
            Scroll = 0;
        else
            Scroll = Scroll.Clamp(0, _buyItemsList.Count - 6);

        if (_buyItemsList.Count < 6)
            Cursor = Cursor.Clamp(0, _buyItemsList.Count - 1);
        else
            Cursor = Cursor.Clamp(0, 5);

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = Scroll; i <= Scroll + 5; i++)
            {
                if (i <= _buyItemsList.Count - 1)
                {
                    if (new Rectangle(100, 100 + (i - Scroll) * 96, 64 * 7, 64).Contains(MouseHandler.MousePosition) == true)
                        Cursor = i - Scroll;
                }
            }
            if (new Rectangle(736, 160, 256, 256).Contains(MouseHandler.MousePosition) == true)
                _buyItemsShowDescription = !_buyItemsShowDescription;
            if (new Rectangle(664, 484, 64, 64).Contains(MouseHandler.MousePosition) == true)
                ButtonBuyItemsMinus();
            if (new Rectangle(856, 484, 64, 64).Contains(MouseHandler.MousePosition) == true)
                ButtonBuyItemsPlus();
            if (new Rectangle(664 + 32, 484 + 64 + 22, 64 * 3, 64).Contains(MouseHandler.MousePosition) == true)
                ButtonBuyItemsBuy();
        }

        if (Cursor > _buyItemsList.Count - 1)
            Cursor = _buyItemsList.Count - 1;

        if (ControllerHandler.ButtonPressed(Buttons.Back) == true || KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true)
            _buyItemsShowDescription = !_buyItemsShowDescription;

        if (_buyItemsList.Count > 0)
            _buyItemsAmount = _buyItemsAmount.Clamp(0, GetMaxBuyItemAmount(_buyItemsList[Scroll + Cursor]));

        if (Controls.Accept(false, true, true) == true)
            ButtonBuyItemsBuy();

        if (Controls.Dismiss(true, true, true) == true)
        {
            if (_buyItemsShowDescription == true)
            {
                SoundManager.PlaySound("select");
                _buyItemsShowDescription = false;
            }
            else
            {
                SoundManager.PlaySound("select");
                _menuState = MenuStates.BuyItemsCategory;
            }
        }

        _buySellSparkleRotation += 0.005F;

        if (_buySellItemShrinking == true)
        {
            _buySellItemSize -= 0.5F;
            if (_buySellItemSize <= 160.0F)
                _buySellItemShrinking = false;
        }
        else
        {
            _buySellItemSize += 0.5F;
            if (_buySellItemSize >= 192.0F)
                _buySellItemShrinking = true;
        }
    }

    private void ButtonBuyItemsMinus()
    {
        if (Controls.ShiftDown() == true)
            _buyItemsAmount -= 5;
        else
            _buyItemsAmount -= 1;
    }

    private void ButtonBuyItemsPlus()
    {
        if (Controls.ShiftDown() == true)
            _buyItemsAmount += 5;
        else
            _buyItemsAmount += 1;
    }

    private void ButtonBuyItemsBuy()
    {
        if (_buyItemsAmount > 0)
        {
            TradeItem tradeItem = _buyItemsList[Scroll + Cursor];

            ChangeCurrencyAmount(-(tradeItem.Price * _buyItemsAmount));
            Core.Player.Inventory.AddItem(tradeItem.ItemID, _buyItemsAmount);

            if (tradeItem.ItemID == 5.ToString() && _buyItemsAmount >= 10)
                Core.Player.Inventory.AddItem(3.ToString(), 1);

            if (tradeItem.Amount > -1)
            {
                for (int i = 0; i < _tradeItems.Count; i++)
                {
                    if (_tradeItems[i].ItemID == tradeItem.ItemID && tradeItem.Amount == _tradeItems[i].Amount)
                    {
                        if (_shopIdentifier != String.Empty)
                        {
                            if (ActionScript.IsRegistered(_shopIdentifier + "_" + tradeItem.ItemID) == false)
                                ActionScript.RegisterID(_shopIdentifier + "_" + tradeItem.ItemID, "int", "0");

                            Object[] registerContent = ActionScript.GetRegisterValue(_shopIdentifier + "_" + tradeItem.ItemID);
                            ActionScript.ChangeRegister(_shopIdentifier + "_" + tradeItem.ItemID,
                                ((int)registerContent[0] + _buyItemsAmount).ToString());
                        }

                        TradeItem t = _tradeItems[i];
                        t.Amount -= _buyItemsAmount;

                        if (t.Amount < 1)
                            _tradeItems.RemoveAt(i);
                        else
                            _tradeItems[i] = t;

                        break;
                    }
                }
            }

            LoadBuyItemsList();
            SoundManager.PlaySound("buy");

            if (_buyItemsList.Count == 0)
            {
                _menuState = MenuStates.BuyItemsCategory;
                Cursor -= 1;
                LoadBuyCategoriesItems();
            }
        }
    }

    private int GetMaxBuyItemAmount(TradeItem tradeItem)
    {
        Items.Item item = tradeItem.GetItem();
        String itemID;
        if (item.IsGameModeItem == true)
            itemID = item.gmID;
        else
            itemID = item.ID.ToString();

        int maxAmount = item.MaxStack - Core.Player.Inventory.GetItemAmount(itemID);

        if (maxAmount > tradeItem.Amount && tradeItem.Amount > -1)
            maxAmount = tradeItem.Amount;

        if (tradeItem.Price == 0)
            return maxAmount;

        int money = GetCurrencyAmount();
        int amount = (int)Math.Floor((double)money / tradeItem.Price);
        return amount.Clamp(0, maxAmount);
    }

    private void DrawBuyItems()
    {
        for (int i = Scroll; i <= Scroll + 5; i++)
        {
            if (i <= _buyItemsList.Count - 1)
            {
                int p = i - Scroll;
                Items.Item item = _buyItemsList[i].GetItem();
                String itemName = item.OneLineName();
                if (item.ItemType == Items.ItemTypes.Machines)
                {
                    if (item.IsGameModeItem == false)
                        itemName += " " + ((Items.TechMachine)item).Attack.Name;
                    else
                        itemName += " " + ((Items.GameModeItem)item).gmTeachMove.Name;
                }
                DrawButton(new Vector2(100, 100 + p * 96), 5, itemName, 16, item.Texture);
            }
        }

        if (_buyItemsList.Count > 0)
        {
            String descriptionHint = ScriptVersion2.ScriptCommander.Parse(Localization.GetString("shop_screen_buysell_DescriptionHint",
                "Press [<system.button(special)>] or Select to view the item's description."))?.ToString() ?? String.Empty;
            Core.SpriteBatch.DrawString(FontManager.InGameFont, descriptionHint,
                new Vector2((int)(Core.windowSize.Width - 64 - FontManager.InGameFont.MeasureString(descriptionHint).X) + 2, 36 + 2), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, descriptionHint,
                new Vector2((int)(Core.windowSize.Width - 64 - FontManager.InGameFont.MeasureString(descriptionHint).X), 36), Color.White);

            while (_buyItemsList.Count <= Scroll + Cursor)
                Cursor -= 1;

            TradeItem selectedItem = _buyItemsList[Scroll + Cursor];

            Core.SpriteBatch.EndBatch();
            Core.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Box\\Sparkle"),
                new Rectangle(736 + 128, 160 + 128, 256, 256), null, Color.White,
                _buySellSparkleRotation, new Vector2(128, 128), SpriteEffects.None, 0.0F);
            Core.SpriteBatch.End();
            Core.SpriteBatch.BeginBatch();

            float itemOffset = (256 - _buySellItemSize) / 2.0F;
            Core.SpriteBatch.Draw(selectedItem.GetItem().Texture,
                new Rectangle((int)(736 + itemOffset), (int)(160 + itemOffset), (int)_buySellItemSize, (int)_buySellItemSize), Color.White);

            if (_buyItemsShowDescription == true)
            {
                Canvas.DrawRectangle(new Rectangle(736 + 28 - 32, 160 + 28, 264, 200), new Color(0, 0, 0, 200));
                String t = selectedItem.GetItem().GetDescription().CropStringToWidth(FontManager.InGameFont, 244);
                Core.SpriteBatch.DrawString(FontManager.InGameFont, t, new Vector2(736 - 2, 160 + 30), Color.White);
            }

            String amount = Core.Player.Inventory.GetItemAmount(selectedItem.ItemID).ToString();
            while (amount.Length < 3)
                amount = "0" + amount;
            String bannerText = String.Empty;
            if (selectedItem.Amount > -1)
                bannerText = " | " + Localization.GetString("shop_screen_buy_InStock", "In Stock:") + " " + selectedItem.Amount;
            DrawBanner(new Vector2(664, 430), 30,
                Localization.GetString("shop_screen_buysell_InInventory", "In Inventory:") + " " + amount + bannerText,
                FontManager.MainFont, 400);

            Core.SpriteBatch.Draw(_texture, new Rectangle(664, 484, 64, 64), new Rectangle(16, 32, 16, 16), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, "-", new Vector2(664 + 23, 484 + 2), Color.Black, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

            Canvas.DrawRectangle(new Rectangle(740, 492, 104, 48), new Color(77, 147, 198));
            Canvas.DrawRectangle(new Rectangle(744, 496, 96, 40), new Color(232, 240, 248));
            String amountString = _buyItemsAmount.ToString();
            while (amountString.Length < 3)
                amountString = "0" + amountString;
            amountString = "x" + amountString;
            Core.SpriteBatch.DrawString(FontManager.MainFont, amountString,
                new Vector2(792 - FontManager.MainFont.MeasureString(amountString).X / 2.0F, 504), Color.Black);

            Core.SpriteBatch.Draw(_texture, new Rectangle(856, 484, 64, 64), new Rectangle(16, 32, 16, 16), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, "+", new Vector2(856 + 19, 484 + 6), Color.Black, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

            Core.SpriteBatch.DrawString(FontManager.MainFont,
                Localization.GetString("shop_screen_buysell_PricePerItem", "Per Item:") + " " + selectedItem.Price.ToString() + GetCurrencyShort() + Environment.NewLine +
                Localization.GetString("shop_screen_buysell_PriceTotal", "Total:") + " " + (_buyItemsAmount * selectedItem.Price).ToString() + GetCurrencyShort(),
                new Vector2(930, 490), Color.White);

            if (_buyItemsAmount > 0)
            {
                DrawButton(new Vector2(664 + 32, 484 + 64 + 22), 1, Localization.GetString("shop_screen_button_buy", "Buy"), 64);
                if (ControllerHandler.IsConnected() == true)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\GamePad\\xboxControllerButtonA"),
                        new Rectangle(664 + 48, 484 + 64 + 34, 40, 40), Color.White);
            }
        }

        DrawBanner(new Vector2(664, 110), 30,
            Localization.GetString("shop_screen_buysell_CurrentBalance", "Current balance:") + " " + GetCurrencyDisplay(),
            FontManager.MainFont, 400);

        DrawMainCursor();
    }

    #endregion

    #region SellCategoryScreen

    private List<Items.ItemTypes> _loadedSellCategories = [];

    private void LoadSellCategoryItems()
    {
        _loadedSellCategories.Clear();
        foreach (PlayerInventory.ItemContainer c in Core.Player.Inventory)
        {
            Items.Item i = Items.Item.GetItemByID(c.ItemID);
            if (_loadedSellCategories.Contains(i.ItemType) == false && i.CanBeTraded == true)
                _loadedSellCategories.Add(i.ItemType);
        }
        _loadedSellCategories = _loadedSellCategories.OrderBy(c => (int)c).ToList();
    }

    private void UpdateSellCategory()
    {
        _title = Localization.GetString("shop_screen_title_SellItems", "Sell Items");

        if (_loadedSellCategories.Count > 0)
        {
            if (Controls.Down(true, true, true, true, true, true) == true)
            {
                Cursor += 1;
                if (Controls.ShiftDown() == true)
                    Cursor += 4;
            }
            if (Controls.Up(true, true, true, true, true, true) == true)
            {
                Cursor -= 1;
                if (Controls.ShiftDown() == true)
                    Cursor -= 4;
            }

            while (Cursor > 5) { Cursor -= 1; Scroll += 1; }
            while (Cursor < 0) { Cursor += 1; Scroll -= 1; }

            if (_loadedSellCategories.Count < 7)
                Scroll = 0;
            else
                Scroll = Scroll.Clamp(0, _loadedSellCategories.Count - 6);

            if (_loadedSellCategories.Count < 6)
                Cursor = Cursor.Clamp(0, _loadedSellCategories.Count - 1);
            else
                Cursor = Cursor.Clamp(0, 5);

            if (Controls.Accept(true, false, false) == true)
            {
                for (int i = Scroll; i <= Scroll + 5; i++)
                {
                    if (i <= _loadedSellCategories.Count - 1)
                    {
                        if (new Rectangle(100, 100 + (i - Scroll) * 96, 64 * 7, 64).Contains(MouseHandler.MousePosition) == true)
                        {
                            if (i == Scroll + Cursor)
                            {
                                SoundManager.PlaySound("select");
                                ButtonSellCategoriesAccept();
                            }
                            else
                            {
                                Cursor = i - Scroll;
                            }
                        }
                    }
                }
            }

            if (Controls.Accept(false, true, true) == true)
            {
                SoundManager.PlaySound("select");
                ButtonSellCategoriesAccept();
            }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            SoundManager.PlaySound("select");
            _menuState = MenuStates.MainPage;
        }
    }

    private void ButtonSellCategoriesAccept()
    {
        _currentCategory = _loadedSellCategories[Cursor + Scroll];
        _menuState = MenuStates.SellItems;
        Cursor = 0;
        Scroll = 0;
        LoadSellItemsList();
    }

    private void DrawSellCategory()
    {
        if (_loadedSellCategories.Count > 0)
        {
            for (int i = Scroll; i <= Scroll + 5; i++)
            {
                if (i <= _loadedSellCategories.Count - 1)
                {
                    int p = i - Scroll;
                    DrawButton(new Vector2(100, 100 + p * 96), 5,
                        Localization.GetString("item_category_" + _loadedSellCategories[i].ToString(), _loadedSellCategories[i].ToString()),
                        16, GetItemTypeTexture(_loadedSellCategories[i]));
                }
            }
            DrawMainCursor();
        }
        else
        {
            DrawBanner(new Vector2((float)(Core.windowSize.Width / 2 - 250), (float)(Core.windowSize.Height / 2 - 50)), 100,
                Localization.GetString("shop_screen_sell_NoItemsToSell", "You have no items to sell."), FontManager.MainFont, 500);
        }
    }

    #endregion

    #region SellItemsScreen

    private List<TradeItem> _sellItemsList = [];
    private int _sellItemsAmount = 1;
    private bool _sellItemsShowDescription = false;

    private void LoadSellItemsList()
    {
        _sellItemsList.Clear();
        foreach (PlayerInventory.ItemContainer c in Core.Player.Inventory)
        {
            Items.Item i = Items.Item.GetItemByID(c.ItemID);
            String itemID;
            if (i.IsGameModeItem == true)
                itemID = i.gmID;
            else
                itemID = i.ID.ToString();

            if (i.CanBeTraded == true && i.ItemType == _currentCategory)
            {
                int price = -1;
                foreach (TradeItem sellItem in _possibleStoreItems)
                {
                    if (sellItem.ItemID == itemID)
                        price = sellItem.Price;
                }
                _sellItemsList.Add(new TradeItem(itemID, c.Amount, price, _currency));
            }
        }
        _sellItemsList = _sellItemsList.OrderBy(i => i.GetItem().OneLineName()).ToList();
    }

    private void UpdateSellItems()
    {
        _title = Localization.GetString("shop_screen_title_SellCategory", "Sell [CATEGORY]")
            .Replace("[CATEGORY]", Localization.GetString("item_category_" + _currentCategory.ToString(), _currentCategory.ToString()));

        if (Controls.Down(true, true, true, true, true, true) == true)
        {
            Cursor += 1;
            if (Controls.ShiftDown() == true)
                Cursor += 4;
        }
        if (Controls.Up(true, true, true, true, true, true) == true)
        {
            Cursor -= 1;
            if (Controls.ShiftDown() == true)
                Cursor -= 4;
        }
        if (Controls.Right(true, true, false, true, true, true) == true)
        {
            _sellItemsAmount += 1;
            if (Controls.ShiftDown() == true)
                _sellItemsAmount += 4;
        }
        if (Controls.Left(true, true, false, true, true, true) == true)
        {
            _sellItemsAmount -= 1;
            if (Controls.ShiftDown() == true)
                _sellItemsAmount -= 4;
        }

        SellItemsClampCursor();

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = Scroll; i <= Scroll + 5; i++)
            {
                if (i <= _sellItemsList.Count - 1)
                {
                    if (new Rectangle(100, 100 + (i - Scroll) * 96, 64 * 7, 64).Contains(MouseHandler.MousePosition) == true)
                        Cursor = i - Scroll;
                }
            }
            if (new Rectangle(736, 160, 256, 256).Contains(MouseHandler.MousePosition) == true)
                _sellItemsShowDescription = !_sellItemsShowDescription;
            if (new Rectangle(664, 484, 64, 64).Contains(MouseHandler.MousePosition) == true)
                ButtonSellItemsMinus();
            if (new Rectangle(856, 484, 64, 64).Contains(MouseHandler.MousePosition) == true)
                ButtonSellItemsPlus();
            if (new Rectangle(664 + 32, 484 + 64 + 22, 64 * 3, 64).Contains(MouseHandler.MousePosition) == true)
            {
                SoundManager.PlaySound("select");
                ButtonSellItemsSell();
            }
        }

        if (ControllerHandler.ButtonPressed(Buttons.Back) == true || KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true)
            _sellItemsShowDescription = !_sellItemsShowDescription;

        if (_sellItemsList.Count > 0)
            _sellItemsAmount = _sellItemsAmount.Clamp(0, _sellItemsList[Scroll + Cursor].Amount);

        if (Controls.Accept(false, true, true) == true)
        {
            SoundManager.PlaySound("select");
            ButtonSellItemsSell();
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            if (_sellItemsShowDescription == true)
            {
                _sellItemsShowDescription = false;
                SoundManager.PlaySound("select");
            }
            else
            {
                _menuState = MenuStates.SellItemsCategory;
                SoundManager.PlaySound("select");
            }
        }

        _buySellSparkleRotation += 0.005F;

        if (_buySellItemShrinking == true)
        {
            _buySellItemSize -= 0.5F;
            if (_buySellItemSize <= 160.0F)
                _buySellItemShrinking = false;
        }
        else
        {
            _buySellItemSize += 0.5F;
            if (_buySellItemSize >= 192.0F)
                _buySellItemShrinking = true;
        }
    }

    private void SellItemsClampCursor()
    {
        while (Cursor > 5) { Cursor -= 1; Scroll += 1; }
        while (Cursor < 0) { Cursor += 1; Scroll -= 1; }

        if (_sellItemsList.Count < 7)
            Scroll = 0;
        else
            Scroll = Scroll.Clamp(0, _sellItemsList.Count - 6);

        if (_sellItemsList.Count < 6)
            Cursor = Cursor.Clamp(0, _sellItemsList.Count - 1);
        else
            Cursor = Cursor.Clamp(0, 5);
    }

    private void ButtonSellItemsMinus()
    {
        if (Controls.ShiftDown() == true)
            _sellItemsAmount -= 5;
        else
            _sellItemsAmount -= 1;
    }

    private void ButtonSellItemsPlus()
    {
        if (Controls.ShiftDown() == true)
            _sellItemsAmount += 5;
        else
            _sellItemsAmount += 1;
    }

    private void ButtonSellItemsSell()
    {
        if (_sellItemsAmount > 0)
        {
            _sellItemsConfirmationCursor = 0;
            _menuState = MenuStates.SellItemsConfirmation;
        }
    }

    private void DrawSellItems()
    {
        for (int i = Scroll; i <= Scroll + 5; i++)
        {
            if (i <= _sellItemsList.Count - 1)
            {
                int p = i - Scroll;
                Items.Item item = _sellItemsList[i].GetItem();
                String itemName = item.OneLineName();
                if (item.ItemType == Items.ItemTypes.Machines)
                {
                    if (item.IsGameModeItem == false)
                        itemName += " " + ((Items.TechMachine)item).Attack.Name;
                    else
                        itemName += " " + ((Items.GameModeItem)item).gmTeachMove.Name;
                }
                DrawButton(new Vector2(100, 100 + p * 96), 5, itemName, 16, item.Texture);
            }
        }

        if (_sellItemsList.Count > 0)
        {
            String descriptionHint = ScriptVersion2.ScriptCommander.Parse(Localization.GetString("shop_screen_buysell_DescriptionHint",
                "Press [<system.button(special)>] or Select to view the item's description."))?.ToString() ?? String.Empty;
            Core.SpriteBatch.DrawString(FontManager.InGameFont, descriptionHint,
                new Vector2((int)(Core.windowSize.Width - 64 - FontManager.InGameFont.MeasureString(descriptionHint).X) + 2, 36 + 2), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, descriptionHint,
                new Vector2((int)(Core.windowSize.Width - 64 - FontManager.InGameFont.MeasureString(descriptionHint).X), 36), Color.White);

            TradeItem selectedItem = _sellItemsList[Scroll + Cursor];

            Core.SpriteBatch.EndBatch();
            Core.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Box\\Sparkle"),
                new Rectangle(736 + 128, 160 + 128, 256, 256), null, Color.White,
                _buySellSparkleRotation, new Vector2(128, 128), SpriteEffects.None, 0.0F);
            Core.SpriteBatch.End();
            Core.SpriteBatch.BeginBatch();

            float itemOffset = (256 - _buySellItemSize) / 2.0F;
            Core.SpriteBatch.Draw(selectedItem.GetItem().Texture,
                new Rectangle((int)(736 + itemOffset), (int)(160 + itemOffset), (int)_buySellItemSize, (int)_buySellItemSize), Color.White);

            if (_sellItemsShowDescription == true)
            {
                Canvas.DrawRectangle(new Rectangle(736 + 28 - 32, 160 + 28, 264, 200), new Color(0, 0, 0, 200));
                String t = selectedItem.GetItem().GetDescription().CropStringToWidth(FontManager.InGameFont, 244);
                Core.SpriteBatch.DrawString(FontManager.InGameFont, t, new Vector2(736 - 2, 160 + 30), Color.White);
            }

            String amount = Core.Player.Inventory.GetItemAmount(selectedItem.ItemID).ToString();
            while (amount.Length < 3)
                amount = "0" + amount;
            DrawBanner(new Vector2(664, 430), 30,
                Localization.GetString("shop_screen_buysell_InInventory", "In Inventory:") + " " + amount,
                FontManager.MainFont, 400);

            Core.SpriteBatch.Draw(_texture, new Rectangle(664, 484, 64, 64), new Rectangle(16, 32, 16, 16), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, "-", new Vector2(664 + 23, 484 + 2), Color.Black, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

            Canvas.DrawRectangle(new Rectangle(740, 492, 104, 48), new Color(77, 147, 198));
            Canvas.DrawRectangle(new Rectangle(744, 496, 96, 40), new Color(232, 240, 248));
            String amountString = _sellItemsAmount.ToString();
            while (amountString.Length < 3)
                amountString = "0" + amountString;
            amountString = "x" + amountString;
            Core.SpriteBatch.DrawString(FontManager.MainFont, amountString,
                new Vector2(792 - FontManager.MainFont.MeasureString(amountString).X / 2.0F, 504), Color.Black);

            Core.SpriteBatch.Draw(_texture, new Rectangle(856, 484, 64, 64), new Rectangle(16, 32, 16, 16), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, "+", new Vector2(856 + 19, 484 + 6), Color.Black, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

            Core.SpriteBatch.DrawString(FontManager.MainFont,
                Localization.GetString("shop_screen_buysell_PricePerItem", "Per Item:") + " " + selectedItem.SellPrice().ToString() + GetCurrencyShort() + Environment.NewLine +
                Localization.GetString("shop_screen_buysell_PriceTotal", "Total:") + " " + (_sellItemsAmount * selectedItem.SellPrice()).ToString() + GetCurrencyShort(),
                new Vector2(930, 490), Color.White);

            if (_sellItemsAmount > 0)
            {
                DrawButton(new Vector2(664 + 32, 484 + 64 + 22), 1, Localization.GetString("shop_screen_button_sell", "Sell"), 64);
                if (ControllerHandler.IsConnected() == true)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\GamePad\\xboxControllerButtonA"),
                        new Rectangle(664 + 48, 484 + 64 + 34, 40, 40), Color.White);
            }
        }

        DrawBanner(new Vector2(664, 110), 30,
            Localization.GetString("shop_screen_buysell_CurrentBalance", "Current balance:") + " " + GetCurrencyDisplay(),
            FontManager.MainFont, 400);

        DrawMainCursor();
    }

    #endregion

    #region SellItemsConfirmationScreen

    private int _sellItemsConfirmationCursor = 0;

    private void UpdateSellConfirmation()
    {
        if (Controls.Down(true, true, true, true, true, true) == true)
            _sellItemsConfirmationCursor += 1;
        if (Controls.Up(true, true, true, true, true, true) == true)
            _sellItemsConfirmationCursor -= 1;

        _sellItemsConfirmationCursor = _sellItemsConfirmationCursor.Clamp(0, 1);

        if (Controls.Accept(true, false, false) == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (new Rectangle((int)(Core.windowSize.Width / 2.0F - 192), (int)(Core.windowSize.Height / 2.0F - 60 + (i * 96)), 64 * 5, 64).Contains(MouseHandler.MousePosition) == true)
                {
                    if (_sellItemsConfirmationCursor == i)
                    {
                        if (i == 0)
                            ButtonSellConfirmationSell();
                        else
                            ButtonSellConfirmationCancel();
                    }
                    else
                    {
                        _sellItemsConfirmationCursor = i;
                    }
                }
            }
        }

        if (Controls.Accept(false, true, true) == true)
        {
            if (_sellItemsConfirmationCursor == 0)
                ButtonSellConfirmationSell();
            else
                ButtonSellConfirmationCancel();
        }
    }

    private void DrawSellConfirmation()
    {
        DrawSellItems();

        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 300), (int)(Core.windowSize.Height / 2 - 200), 600, 400), new Color(0, 0, 0, 150));

        TradeItem tradeItem = _sellItemsList[Scroll + Cursor];

        String itemName = tradeItem.GetItem().OneLineName();
        if (_sellItemsAmount > 1)
            itemName = tradeItem.GetItem().OneLinePluralName();

        String text = Localization.GetString("shop_screen_sell_confirmation", "Do you want to sell~[AMOUNT] [ITEM]?")
            .Replace("~", Environment.NewLine).Replace("*", Environment.NewLine)
            .Replace("[AMOUNT]", _sellItemsAmount.ToString())
            .Replace("[ITEM]", itemName);

        Core.SpriteBatch.DrawString(FontManager.MainFont, text,
            new Vector2(Core.windowSize.Width / 2.0F - FontManager.MainFont.MeasureString(text).X, Core.windowSize.Height / 2.0F - 170),
            Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

        DrawButton(new Vector2(Core.windowSize.Width / 2.0F - 192, Core.windowSize.Height / 2.0F - 60), 4,
            Localization.GetString("shop_screen_button_sell", "Sell"), 16, null);
        DrawButton(new Vector2(Core.windowSize.Width / 2.0F - 192, Core.windowSize.Height / 2.0F + 36), 4,
            Localization.GetString("global_cancel", "Cancel"), 16, null);

        Vector2 cPosition = new Vector2(Core.windowSize.Width / 2.0F - 192 + 280, Core.windowSize.Height / 2.0F - 60 + _sellItemsConfirmationCursor * 96 - 42);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)cPosition.X, (int)cPosition.Y, 64, 64), Color.White);
    }

    private void ButtonSellConfirmationSell()
    {
        TradeItem tradeItem = _sellItemsList[Scroll + Cursor];

        ChangeCurrencyAmount(tradeItem.SellPrice() * _sellItemsAmount);
        Core.Player.Inventory.RemoveItem(tradeItem.ItemID, _sellItemsAmount);
        LoadSellItemsList();
        SellItemsClampCursor();
        SoundManager.PlaySound("buy");

        if (_sellItemsList.Count == 0)
        {
            _menuState = MenuStates.SellItemsCategory;
            LoadSellCategoryItems();
        }
        else
        {
            _menuState = MenuStates.SellItems;
        }
    }

    private void ButtonSellConfirmationCancel()
    {
        _menuState = MenuStates.SellItems;
    }

    #endregion

    private void DrawButton(Vector2 position, int width, String text, int textOffset, Texture2D? image = null)
    {
        Core.SpriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
        Core.SpriteBatch.Draw(_texture, new Rectangle((int)position.X + 64, (int)position.Y, 64 * width, 64), new Rectangle(32, 16, 16, 16), Color.White);
        Core.SpriteBatch.Draw(_texture, new Rectangle((int)position.X + 64 * (width + 1), (int)position.Y, 64, 64), new Rectangle(16, 16, 16, 16), Color.White,
            0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

        if (image == null)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, text,
                new Vector2(textOffset + (int)position.X, (int)position.Y + 16), Color.Black, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, text,
                new Vector2(4 + 16 + image.Width + textOffset + (int)position.X, (int)position.Y + 16), Color.Black, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
            Core.SpriteBatch.Draw(image, new Rectangle((int)position.X + textOffset + 8, (int)position.Y + 19, image.Width, image.Height), Color.White);
        }
    }

    private void DrawBanner(Vector2 position, int height, String text, SpriteFont font, int? fixedWidth = null)
    {
        float textWidth = font.MeasureString(text).X;
        float textHeight = font.MeasureString(text).Y;
        float textY = (height / 2.0F) - (textHeight / 2.0F);
        int width = (int)((height + 10) * 2 + textWidth);

        if (fixedWidth.HasValue == true)
            width = fixedWidth.GetValueOrDefault();

        Canvas.DrawGradient(new Rectangle((int)position.X, (int)position.Y, height, height), new Color(42, 167, 198, 0), new Color(42, 167, 198, 150), true, -1);
        Canvas.DrawRectangle(new Rectangle((int)position.X + height, (int)position.Y, width - (height * 2), height), new Color(42, 167, 198, 150));
        Canvas.DrawGradient(new Rectangle((int)position.X + (width - height), (int)position.Y, height, height), new Color(42, 167, 198, 150), new Color(42, 167, 198, 0), true, -1);

        Core.SpriteBatch.DrawString(font, text,
            new Vector2((int)(position.X + (width / 2) - font.MeasureString(text).X / 2), position.Y + textY), Color.White);
    }

    private Texture2D GetItemTypeTexture(Items.ItemTypes itemType)
    {
        int i = 0;
        switch (itemType)
        {
            case Items.ItemTypes.Standard: i = 0; break;
            case Items.ItemTypes.Medicine: i = 1; break;
            case Items.ItemTypes.Machines: i = 2; break;
            case Items.ItemTypes.Pokeballs: i = 3; break;
            case Items.ItemTypes.Plants: i = 4; break;
            case Items.ItemTypes.Mail: i = 5; break;
            case Items.ItemTypes.BattleItems: i = 6; break;
            case Items.ItemTypes.KeyItems: i = 7; break;
        }
        return TextureManager.GetTexture(TextureManager.GetTexture("GUI\\Menus\\BagPack"), new Rectangle(i * 24, 150, 24, 24));
    }

    private int GetCurrencyAmount()
    {
        switch (_currency)
        {
            case Currencies.BattlePoints: return Core.Player.BP;
            case Currencies.Coins: return Core.Player.Coins;
            case Currencies.Pokédollar: return Core.Player.Money;
        }
        return 0;
    }

    private String GetCurrencyDisplay()
    {
        switch (_currency)
        {
            case Currencies.BattlePoints:
                return GetCurrencyAmount().ToString() + " " + Localization.GetString("shop_screen_currency_BattlePoints", "Battle Points");
            case Currencies.Coins:
                return GetCurrencyAmount().ToString() + " " + Localization.GetString("shop_screen_currency_Coins", "Coins");
            case Currencies.Pokédollar:
                return GetCurrencyAmount().ToString() + " " + Localization.GetString("shop_screen_currency_Pokédollars", "$");
        }
        return String.Empty;
    }

    private String GetCurrencyShort()
    {
        switch (_currency)
        {
            case Currencies.BattlePoints: return Localization.GetString("shop_screen_currency_short_BattlePoints", "BP");
            case Currencies.Coins: return Localization.GetString("shop_screen_currency_short_Coins", "C");
            case Currencies.Pokédollar: return Localization.GetString("shop_screen_currency_short_Pokédollars", "$");
        }
        return String.Empty;
    }

    private void ChangeCurrencyAmount(int change)
    {
        switch (_currency)
        {
            case Currencies.BattlePoints:
                Core.Player.BP = (Core.Player.BP + change).Clamp(0, int.MaxValue);
                break;
            case Currencies.Coins:
                Core.Player.Coins = (Core.Player.Coins + change).Clamp(0, int.MaxValue);
                break;
            case Currencies.Pokédollar:
                Core.Player.Money = (Core.Player.Money + change).Clamp(0, int.MaxValue);
                break;
        }
    }
}
