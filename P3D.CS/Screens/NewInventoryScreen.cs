using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D.Screens.UI;

namespace P3D;

/// <summary>
/// Displays the inventory and gives the player options to choose and use items.
/// </summary>
public class NewInventoryScreen : Screen, ISelectionScreen
{
    private RenderTarget2D _target1 = null!;
    private RenderTarget2D _target2 = null!;
    private CoreSpriteBatch _itemBatch = null!;
    private CoreSpriteBatch _infoBatch = null!;

    private Texture2D _texture = null!;
    private Texture2D _menuTexture = null!;

    private int _tabIndex = 0;
    private bool _tabInControl = true;

    private int[] _itemindex = [0, 0, 0, 0, 0, 0, 0, 0];
    private int[] _pageIndex = [0, 0, 0, 0, 0, 0, 0, 0];

    private bool _tossingItems = false;
    private int _tossValue = 1;

    private RenderTarget2D? _preScreenTexture;
    private RenderTarget2D _preScreenTarget = null!;
    private Identifications[] _blurScreens =
    {
        Identifications.BattleScreen,
        Identifications.OverworldScreen,
        Identifications.MailSystemScreen
    };

    private int ItemIndex
    {
        get => _itemindex[_tabIndex];
        set => _itemindex[_tabIndex] = value;
    }

    private int PageIndex
    {
        get => _pageIndex[_tabIndex];
        set => _pageIndex[_tabIndex] = value;
    }

    private int[] _tabHighlight = [0, 0, 0, 0, 0, 0, 0, 0];

    private float _interfaceFade = 0f;
    private bool _closing = false;
    private float _enrollY = 0f;
    private float _itemIntro = 0f;

    private class ItemAnimation
    {
        public float _shakeV;
        public bool _shakeLeft;
        public int _shakeCount;
    }
    private ItemAnimation _itemAnimation = new ItemAnimation();

    private bool _isInfoShowing = false;

    private int _infoSide = 0;
    private int _infoSize = 0;
    private int _infoSizeTarget = 0;
    private int _infoPosition = 0;
    private int _infoPositionTarget = 0;

    private int _itemColumnLeft = 0;
    private int _itemColumnLeftOffset = 0;
    private int _itemColumnLeftOffsetTarget = 0;
    private int _itemColumnRightOffset = 0;
    private int _itemColumnRightOffsetTarget = 0;

    private const String INFO_ITEM_OPTION_USE = "USE";
    private const String INFO_ITEM_OPTION_GIVE = "GIVE";
    private const String INFO_ITEM_OPTION_TOSS = "TOSS";
    private const String INFO_ITEM_OPTION_SELECT = "SELECT";

    private List<String> _infoItemOptions = [];
    private List<String> _infoItemOptionsNormal = [];
    private int[] _infoItemOptionSize = [0, 0, 0];
    private int _infoItemOptionSelection = 0;

    private PlayerInventory.ItemContainer[] _items = [];

    private float _messageDelay = 0f;
    private String _messageText = String.Empty;

    public static String SelectedItem = String.Empty;
    private bool _doReturnItem = false;

    public delegate void DoStuff(int itemID);
    private DoStuff? _returnItem;

    private int[] _allowedPages = [];
    private List<String>? _allowedItems;

    private Resources.Blur.BlurHandler _blur = null!;

    // ISelectionScreen
    private ISelectionScreen.ScreenMode _mode = ISelectionScreen.ScreenMode.Default;
    private bool _canExit = true;
    private Items.ItemTypes[] _visibleItemTypes = [];

    public event Action<Object[]>? SelectedObject;

    public ISelectionScreen.ScreenMode Mode
    {
        get => _mode;
        set => _mode = value;
    }

    public bool CanExit
    {
        get => _canExit;
        set => _canExit = value;
    }

    public Items.ItemTypes[] VisibleItemTypes
    {
        set => _visibleItemTypes = value;
    }

    public NewInventoryScreen(Screen currentScreen, int[] allowedPages, int startPageIndex,
                              DoStuff? doStuff, List<String>? allowedItems = null, bool doReturnItem = false)
    {
        SelectedItem = String.Empty;
        _preScreenTarget = new RenderTarget2D(Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        _blur = new Resources.Blur.BlurHandler(Core.windowSize.Width, Core.windowSize.Height);

        if (allowedPages.Contains(startPageIndex) == false)
        {
            _tabIndex = allowedPages[0];
        }
        else
        {
            _tabIndex = startPageIndex;
        }

        _pageIndex = Player.Temp.BagPageIndex;
        _itemindex = Player.Temp.BagItemIndex;

        _allowedPages = allowedPages;
        _allowedItems = allowedItems;
        _returnItem = doStuff;
        _doReturnItem = doReturnItem;

        _target1 = new RenderTarget2D(Core.GraphicsDevice, 816, 400 - 32, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
        _target2 = new RenderTarget2D(Core.GraphicsDevice, 500, 368, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        _itemBatch = new CoreSpriteBatch(Core.GraphicsDevice);
        _infoBatch = new CoreSpriteBatch(Core.GraphicsDevice);

        Identification = Identifications.InventoryScreen;
        PreScreen = currentScreen;
        IsDrawingGradients = true;

        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        _menuTexture = TextureManager.GetTexture("GUI\\Menus\\Inventory");

        ResetAnimation();

        _visibleItemTypes =
        [
            Items.ItemTypes.Standard,
            Items.ItemTypes.Medicine,
            Items.ItemTypes.Plants,
            Items.ItemTypes.Pokeballs,
            Items.ItemTypes.Machines,
            Items.ItemTypes.Mail,
            Items.ItemTypes.BattleItems,
            Items.ItemTypes.KeyItems
        ];

        _tabHighlight[_tabIndex] = 255;

        LoadItems();
    }

    public NewInventoryScreen(Screen currentScreen, int[] allowedPages, DoStuff? doStuff,
                              List<String>? allowedItems = null, bool doReturnItem = false)
        : this(currentScreen, allowedPages, Player.Temp.BagIndex, doStuff, allowedItems, doReturnItem)
    {
    }

    public NewInventoryScreen(Screen currentScreen, List<String>? allowedItems = null, bool doReturnItem = false)
        : this(currentScreen, [0, 1, 2, 3, 4, 5, 6, 7], Player.Temp.BagIndex, null, allowedItems, doReturnItem)
    {
    }

    public NewInventoryScreen(Screen currentScreen)
        : this(currentScreen, null, false)
    {
    }

    public void ReturnSelectedItem(String itemID)
    {
        SelectedItem = itemID;
        _closing = true;
    }

    public override void Draw()
    {
        if (_blurScreens.Contains(PreScreen.Identification))
        {
            DrawPrescreen();
        }
        else
        {
            PreScreen.Draw();
        }
        DrawGradients((int)(255 * _interfaceFade));

        DrawMain();
        DrawTabs();

        DrawMessage();

        PokemonImageView.Draw();
        ImageView.Draw();
        TextBox.Draw();
        ChooseBox.Draw();

        DrawAmount();
    }

    private void DrawPrescreen()
    {
        if (_preScreenTexture == null || _preScreenTexture.IsContentLost)
        {
            Core.SpriteBatch.EndBatch();
            RenderTarget2D target = _preScreenTarget;
            Core.GraphicsDevice.SetRenderTarget(target);
            Core.GraphicsDevice.Clear(Core.BackgroundColor);
            Core.SpriteBatch.BeginBatch();
            PreScreen.Draw();
            Core.SpriteBatch.EndBatch();
            Core.GraphicsDevice.SetRenderTarget(null);
            Core.SpriteBatch.BeginBatch();
            _preScreenTexture = target;
        }
        Core.SpriteBatch.Draw(_blur.Perform(_preScreenTexture), Core.windowSize, Color.White);
    }

    private void DrawMessage()
    {
        if (_messageDelay > 0f)
        {
            float textFade = 1.0f;
            if (_messageDelay <= 1.0f)
            {
                textFade = _messageDelay;
            }

            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 150), (int)(Core.windowSize.Height - 200), 300, 100), new Color(0, 0, 0, (int)(150 * textFade * _interfaceFade)));

            String text = _messageText.CropStringToWidth(FontManager.ChatFont, 250);
            Vector2 size = FontManager.ChatFont.MeasureString(text);

            Core.SpriteBatch.DrawString(FontManager.ChatFont, text, new Vector2((float)(Core.windowSize.Width / 2 - size.X / 2), (float)(Core.windowSize.Height - 150 - size.Y / 2)), new Color(255, 255, 255, (int)(255 * textFade * _interfaceFade)));
        }
    }

    private void DrawTabs()
    {
        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);
        Color mainBackgroundColor = Color.White;
        if (_closing == true)
        {
            mainBackgroundColor = new Color(255, 255, 255, (int)(255 * _interfaceFade));
        }

        for (int x = 0; x <= 368; x += 16)
        {
            int cTabIndex = (int)Math.Floor(x / 48.0);
            Color bgColor = Color.White;

            if (cTabIndex != _tabIndex && cTabIndex < _tabHighlight.Length)
            {
                int gC = 128 + (int)(128 * (_tabHighlight[cTabIndex] / 255f));
                bgColor = new Color(gC, gC, gC);
            }

            if (_closing)
            {
                bgColor = new Color(bgColor.R, bgColor.G, bgColor.B, (int)(bgColor.A * _interfaceFade));
            }

            for (int y = 0; y <= 32; y += 16)
            {
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle(halfWidth - 400 + x, halfHeight - 200 + y, 16, 16), new Rectangle(0, 0, 4, 4), bgColor);
            }
            if (x % 48 == 32)
            {
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle(halfWidth - 400 + x - 24, halfHeight - 200 + 8, 32, 32), GetTabImageRect(cTabIndex), bgColor);
            }
        }

        int tabDescriptionWidth = 176;
        Color tBgColor = new Color(128, 128, 128);
        if (_closing)
        {
            tBgColor = new Color(tBgColor.R, tBgColor.G, tBgColor.B, (int)(tBgColor.A * _interfaceFade));
        }
        for (int x = 0; x <= tabDescriptionWidth; x += 16)
        {
            for (int y = 0; y <= 32; y += 16)
            {
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle(halfWidth - 400 + x + 384, halfHeight - 200 + y, 16, 16), new Rectangle(0, 0, 4, 4), tBgColor);
            }
        }
        Canvas.DrawGradient(Core.SpriteBatch, new Rectangle(halfWidth - 400 + 384 + tabDescriptionWidth + 16, halfHeight - 200, 800 - (384 + tabDescriptionWidth), 48), new Color(0, 0, 0, (int)(tBgColor.A * 0.5f)), new Color(0, 0, 0, (int)(tBgColor.A * 0.00f)), true, -1);

        String tabName = _tabIndex switch
        {
            0 => Localization.GetString("item_category_Standard", "Standard"),
            1 => Localization.GetString("item_category_Medicine", "Medicine"),
            2 => Localization.GetString("item_category_Plants", "Plants"),
            3 => Localization.GetString("item_category_Pokéballs", "Pokéballs"),
            4 => Localization.GetString("item_category_Machines", "TM/HM"),
            5 => Localization.GetString("item_category_Mail", "Mail"),
            6 => Localization.GetString("item_category_BattleItems", "Battle Items"),
            7 => Localization.GetString("item_category_KeyItems", "Key Items"),
            _ => String.Empty
        };

        Color gColor = new Color(164, 164, 164);
        if (_closing)
        {
            gColor = new Color(gColor.R, gColor.G, gColor.B, (int)(gColor.A * _interfaceFade));
        }
        int fontWidth = (int)FontManager.ChatFont.MeasureString(tabName).X;
        Core.SpriteBatch.DrawString(FontManager.ChatFont, tabName, new Vector2(halfWidth - 400 + 384 + (int)((tabDescriptionWidth - fontWidth) * 0.5f), halfHeight - 200 + 12), gColor);
    }

    private void DrawMain()
    {
        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);

        Color mainBackgroundColor = Color.White;
        if (_closing == true)
        {
            mainBackgroundColor = new Color(255, 255, 255, (int)(255 * _interfaceFade));
        }

        Canvas.DrawRectangle(new Rectangle(halfWidth - 400, halfHeight - 232, 260, 32), new Color(ColorProvider.MainColor(false).R, ColorProvider.MainColor(false).G, ColorProvider.MainColor(false).B, mainBackgroundColor.A));
        Canvas.DrawRectangle(new Rectangle(halfWidth - 140, halfHeight - 216, 16, 16), new Color(ColorProvider.MainColor(false).R, ColorProvider.MainColor(false).G, ColorProvider.MainColor(false).B, mainBackgroundColor.A));
        Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 140, halfHeight - 232, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);
        Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 124, halfHeight - 216, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);

        Core.SpriteBatch.DrawString(FontManager.ChatFont, Localization.GetString("inventory_screen_title", "Inventory"), new Vector2(halfWidth - 390, halfHeight - 228), mainBackgroundColor);

        for (int y = 0; y <= (int)_enrollY; y += 16)
        {
            for (int x = 0; x <= 800; x += 16)
            {
                Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + x, halfHeight - 200 + y, 16, 16), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
            }
        }

        int modRes = (int)_enrollY % 16;
        if (modRes > 0)
        {
            for (int x = 0; x <= 800; x += 16)
            {
                Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + x, (int)(_enrollY + (halfHeight - 200)), 16, modRes), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
            }
        }

        if ((int)_enrollY - 32 > 0)
        {
            Core.GraphicsDevice.SetRenderTarget(_target1);
            Core.GraphicsDevice.Clear(Color.Transparent);

            _itemBatch.BeginBatch();

            int itemPanelAlpha = (int)((_tabInControl ? 180 : 255) * _interfaceFade);

            for (int i = 0; i <= 9; i++)
            {
                int iX = (int)Math.Floor(i / 2.0);
                int iY = i % 2;

                if (_items.Length > i + (PageIndex * 10))
                {
                    Items.Item cItem = Items.Item.GetItemByID(_items[i + (PageIndex * 10)].ItemID);

                    if (_itemIntro >= i / 10f)
                    {
                        int yOffset = 0;
                        if (_itemIntro < (i + 1) / 10f)
                        {
                            yOffset = (int)((_itemIntro - (i / 10f)) * 80);
                        }

                        int xOffset = ComputeXOffset(iX);

                        Vector2 itemLoc = new Vector2(iX * 160 + 32 + xOffset, 48 + iY * 160 + 32);

                        Vector2 itemOffset = Vector2.Zero;
                        Size itemSize = new Size(cItem.Texture.Width, cItem.Texture.Height);
                        if (itemSize.Width != 24 || itemSize.Height != 24)
                        {
                            itemOffset.X = (int)(cItem.Texture.Width - 24);
                            itemOffset.Y = (int)(cItem.Texture.Height - 24);
                        }

                        int size = (i == ItemIndex) ? itemSize.Width * 4 : itemSize.Width * 3;

                        _itemBatch.Draw(cItem.Texture, new Rectangle((int)(itemLoc.X + 48 - itemOffset.X), (int)(itemLoc.Y + yOffset - itemOffset.Y), size, size), null, new Color(255, 255, 255, itemPanelAlpha),
                                        (i == ItemIndex) ? _itemAnimation._shakeV : 0f, new Vector2(cItem.Texture.Width / 2.0f), SpriteEffects.None, 0f);

                        int nameTextHeight = 24;
                        if (_tabIndex == 4)
                        {
                            nameTextHeight = 40;
                        }

                        String textLine1 = cItem.Name.GetSplit(0, "~");
                        String textLine2 = String.Empty;

                        int fontSizeOffset = 0;
                        if (cItem.Name.Contains("~"))
                        {
                            fontSizeOffset = 16;
                            textLine2 = cItem.Name.GetSplit(1, "~");
                        }
                        nameTextHeight += fontSizeOffset;

                        Canvas.DrawRectangle(_itemBatch, new Rectangle((int)itemLoc.X - 16 - 9, (int)itemLoc.Y + 48, 128 + 18, nameTextHeight), new Color(0, 0, 0, (int)((_tabInControl ? 64 : 128) * _interfaceFade)));

                        int fontWidth1 = (int)FontManager.MiniFont.MeasureString(textLine1).X;
                        int fontWidth2 = (int)FontManager.MiniFont.MeasureString(textLine2).X;

                        _itemBatch.DrawString(FontManager.MiniFont, textLine1, itemLoc + new Vector2(48 - fontWidth1 / 2.0f, 51), new Color(255, 255, 255, itemPanelAlpha));
                        if (textLine2 != String.Empty)
                        {
                            _itemBatch.DrawString(FontManager.MiniFont, textLine2, itemLoc + new Vector2(48 - fontWidth2 / 2.0f, 51 + 16), new Color(255, 255, 255, itemPanelAlpha));
                        }
                        if (_tabIndex != 7)
                        {
                            _itemBatch.DrawString(FontManager.MainFont, "x" + _items[i + (PageIndex * 10)].Amount.ToString(), itemLoc + new Vector2(84, 26), new Color(40, 40, 40, itemPanelAlpha));
                        }

                        if (_tabIndex == 4)
                        {
                            String attackName;
                            if (cItem.IsGameModeItem == false)
                            {
                                attackName = ((Items.TechMachine)cItem).Attack.Name;
                            }
                            else
                            {
                                attackName = ((Items.GameModeItem)cItem).gmTeachMove.Name;
                            }
                            int tmFontWidth = (int)FontManager.MiniFont.MeasureString(attackName).X;
                            _itemBatch.DrawString(FontManager.MiniFont, attackName, itemLoc + new Vector2(48 - tmFontWidth / 2.0f, 51 + 16 + fontSizeOffset), new Color(255, 255, 255, itemPanelAlpha));
                        }
                    }
                }
            }

            if (_infoSize > 0)
            {
                DrawInfo(_itemBatch, _target1);
            }

            _itemBatch.EndBatch();

            Core.GraphicsDevice.SetRenderTarget(null);

            int drawHeight = 368;
            if (_closing)
            {
                drawHeight = (int)_enrollY - 32;
            }

            Core.SpriteBatch.Draw(_target1, new Rectangle(halfWidth - 400, halfHeight - 200 + 48, 816, drawHeight), mainBackgroundColor);
        }
    }

    private void DrawInfo(SpriteBatch preBatch, RenderTarget2D preTarget)
    {
        if (_items.Length == 0)
        {
            return;
        }

        Core.GraphicsDevice.SetRenderTarget(_target2);
        Core.GraphicsDevice.Clear(Color.Transparent);

        _infoBatch.BeginBatch();
        int alpha = (int)(_infoSize / 500f * 255);

        for (int y = 0; y <= 368; y += 16)
        {
            for (int x = 0; x <= _infoSize + 16; x += 16)
            {
                if (x < _infoSize - 16)
                {
                    _infoBatch.Draw(_menuTexture, new Rectangle(x, y, 16, 16), new Rectangle(0, 0, 4, 4), new Color(128, 128, 128, alpha));
                }
            }
        }

        Canvas.DrawGradient(_infoBatch, new Rectangle(0, 0, 100, 368), new Color(0, 0, 0, alpha), new Color(0, 0, 0, 0), true, -1);
        Canvas.DrawGradient(_infoBatch, new Rectangle(_infoSize - 100, 0, 100, 368), new Color(0, 0, 0, 0), new Color(0, 0, 0, alpha), true, -1);

        int getIndex = ItemIndex + PageIndex * 10;
        Items.Item cItem = Items.Item.GetItemByID(_items[getIndex].ItemID);

        Vector2 itemOffset = Vector2.Zero;
        Size itemSize = new Size(cItem.Texture.Width, cItem.Texture.Height);
        if (itemSize.Width != 24 || itemSize.Height != 24)
        {
            itemOffset.X = (int)(cItem.Texture.Width - 24);
            itemOffset.Y = (int)(cItem.Texture.Height - 24);
        }

        _infoBatch.Draw(cItem.Texture, new Rectangle((int)(24 - itemOffset.X), (int)(24 - itemOffset.Y), itemSize.Width * 2, itemSize.Height * 2), Color.White);

        String itemTitle = cItem.Name;
        String itemSubTitle = cItem.ItemType.ToString();
        String itemDescription = cItem.GetDescription();

        switch (cItem.ItemType)
        {
            case Items.ItemTypes.Machines:
                if (cItem.IsGameModeItem == true)
                {
                    itemTitle = ((Items.GameModeItem)cItem).gmTeachMove.Name;

                    if (((Items.GameModeItem)cItem).gmTeachMove != null)
                    {
                        if (((Items.GameModeItem)cItem).gmIsHM == true)
                        {
                            itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_HM", "Hidden Machine");
                        }
                        else
                        {
                            itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_TM", "Technical Machine");
                        }
                    }

                    itemDescription += Environment.NewLine + ((Items.GameModeItem)cItem).gmTeachMove.Description;
                }
                else
                {
                    Items.TechMachine techMachine = (Items.TechMachine)cItem;

                    itemTitle = techMachine.Attack.Name;

                    if (techMachine.IsTM == false)
                    {
                        itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_HM", "Hidden Machine");
                    }
                    else
                    {
                        itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_TM", "Technical Machine");
                    }

                    itemDescription += Environment.NewLine + techMachine.Attack.Description;
                }
                break;
            case Items.ItemTypes.Standard:
                itemSubTitle = cItem.ItemType.ToString() + Localization.GetString("inventory_screen_ItemSubtitle_Standard_Suffix", "Item");
                break;
            case Items.ItemTypes.KeyItems:
                itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_KeyItem", "Key Item");
                break;
            case Items.ItemTypes.Pokeballs:
                itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_PokeBall", "Poké Ball");
                break;
            case Items.ItemTypes.Plants:
                itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_Plant", "Plant");
                break;
            case Items.ItemTypes.BattleItems:
                itemSubTitle = Localization.GetString("inventory_screen_ItemSubtitle_BattleItem", "Battle Item");
                break;
        }

        _infoBatch.DrawString(FontManager.TextFont, itemTitle.Replace("~", " "), new Vector2(80, 20), Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
        _infoBatch.DrawString(FontManager.TextFont, itemSubTitle, new Vector2(80, 46), Color.LightGray, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
        _infoBatch.DrawString(FontManager.TextFont, itemDescription.CropStringToWidth(FontManager.TextFont, 1.0f, 430), new Vector2(28, 84), Color.LightGray, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

        for (int i = 0; i <= _infoItemOptions.Count - 1; i++)
        {
            Canvas.DrawRectangle(_infoBatch, new Rectangle((int)(250 - _infoItemOptionSize[i] / 2), 158 + i * 64, _infoItemOptionSize[i], 48), new Color(255, 255, 255, 20));
            _infoBatch.DrawString(FontManager.TextFont, _infoItemOptions[i], new Vector2((int)(250 - FontManager.TextFont.MeasureString(_infoItemOptions[i]).X), 168 + i * 64), Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
        }

        _infoBatch.EndBatch();

        Core.GraphicsDevice.SetRenderTarget(preTarget);
        preBatch.Draw(_target2, new Rectangle(_infoPosition + 80, 0, _target2.Width, _target2.Height), new Color(255, 255, 255, alpha));
    }

    private void DrawAmount()
    {
        if (_tossingItems)
        {
            Items.Item cItem = Items.Item.GetItemByID(_items[ItemIndex + PageIndex * 10].ItemID);

            Texture2D canvasTexture = TextureManager.GetTexture(TextureManager.GetTexture("GUI\\Menus\\Menu"), new Rectangle(0, 0, 48, 48));

            String itemID;
            if (cItem.IsGameModeItem == true)
            {
                itemID = cItem.gmID;
            }
            else
            {
                itemID = cItem.ID.ToString();
            }
            String trashText = _tossValue + "/" + Core.Player.Inventory.GetItemAmount(itemID);
            int offsetX = 100;
            int offsetY = Core.windowSize.Height - 390;

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.windowSize.Width / 2) + 180 + offsetX, 240 + offsetY, 128, 64));
            Core.SpriteBatch.DrawString(FontManager.InGameFont, trashText, new Vector2((int)(Core.windowSize.Width / 2) - (FontManager.InGameFont.MeasureString(trashText).X / 2) + 256 + offsetX, 276 + offsetY), Color.Black);
        }
    }

    private int ComputeXOffset(int itemPositionId)
    {
        bool isLeft = _itemColumnLeft >= itemPositionId;
        return isLeft ? _itemColumnLeftOffset : _itemColumnRightOffset;
    }

    private Rectangle GetTabImageRect(int i)
    {
        if (i < 0 || i >= _visibleItemTypes.Length)
        {
            return Rectangle.Empty;
        }
        return _visibleItemTypes[i] switch
        {
            Items.ItemTypes.Standard => new Rectangle(0, 16, 16, 16),
            Items.ItemTypes.Medicine => new Rectangle(16, 16, 16, 16),
            Items.ItemTypes.Plants => new Rectangle(0, 32, 16, 16),
            Items.ItemTypes.Pokeballs => new Rectangle(48, 16, 16, 16),
            Items.ItemTypes.Machines => new Rectangle(32, 16, 16, 16),
            Items.ItemTypes.Mail => new Rectangle(16, 32, 16, 16),
            Items.ItemTypes.BattleItems => new Rectangle(48, 32, 16, 16),
            Items.ItemTypes.KeyItems => new Rectangle(32, 32, 16, 16),
            _ => Rectangle.Empty
        };
    }

    public override void Update()
    {
        for (int index = 0; index <= _tabHighlight.Length - 1; index++)
        {
            if (index != _tabIndex)
            {
                if (_tabHighlight[index] > 0)
                {
                    _tabHighlight[index] -= 15;
                    if (_tabHighlight[index] < 0)
                    {
                        _tabHighlight[index] = 0;
                    }
                }
            }
            else
            {
                _tabHighlight[index] = 255;
            }
        }

        if (_messageDelay > 0f)
        {
            _messageDelay -= 0.1f;
            if (_messageDelay <= 0f)
            {
                _messageDelay = 0f;
            }
        }

        if (_closing)
        {
            if (_interfaceFade > 0f)
            {
                _interfaceFade = MathHelper.Lerp(0, _interfaceFade, 0.8f);
                if (_interfaceFade < 0f)
                {
                    _interfaceFade = 0f;
                }
            }
            if (_enrollY > 0)
            {
                _enrollY = MathHelper.Lerp(0, _enrollY, 0.8f);
                if (_enrollY <= 0)
                {
                    _enrollY = 0;
                }
            }
            if (_enrollY <= 2.0f)
            {
                Core.SetScreen(PreScreen);
            }
        }
        else
        {
            int maxWindowHeight = 400;
            if (_enrollY < maxWindowHeight)
            {
                _enrollY = MathHelper.Lerp(maxWindowHeight, _enrollY, 0.8f);
                if (_enrollY >= maxWindowHeight)
                {
                    _enrollY = maxWindowHeight;
                }
            }
            if (_interfaceFade < 1.0f)
            {
                _interfaceFade = MathHelper.Lerp(1.0f, _interfaceFade, 0.95f);
                if (_interfaceFade > 1.0f)
                {
                    _interfaceFade = 1.0f;
                }
            }
            if (_itemIntro < 1.0f)
            {
                _itemIntro += 0.05f;
                if (_itemIntro > 1.0f)
                {
                    _itemIntro = 1.0f;
                }
            }

            UpdateShakeAnimation();

            if (TextBox.Showing == false && ChooseBox.Showing == false && PokemonImageView.Showing == false && ImageView.Showing == false)
            {
                bool isTabsSelected = _tabInControl;

                UpdateTabs();

                if (isTabsSelected == false)
                {
                    if (_isInfoShowing)
                    {
                        UpdateInfo();
                    }
                    else
                    {
                        UpdateItems();
                    }
                }
            }

            ChooseBox.Update();
            if (ChooseBox.Showing == false)
            {
                TextBox.Update();
            }
            if (PokemonImageView.Showing == true)
            {
                PokemonImageView.Update();
            }
            if (ImageView.Showing == true)
            {
                ImageView.Update();
            }

            UpdateInfoAnimation();
        }

        if (_tossingItems)
        {
            Items.Item cItem = Items.Item.GetItemByID(_items[ItemIndex + PageIndex * 10].ItemID);
            if (Controls.Right(true, true, true, true) || Controls.Up(true, true, true, true))
            {
                _tossValue += 1;
            }
            if (Controls.Left(true, true, true, true) || Controls.Down(true, true, true, true))
            {
                _tossValue -= 1;
            }

            String itemID;
            if (cItem.IsGameModeItem == true)
            {
                itemID = cItem.gmID;
            }
            else
            {
                itemID = cItem.ID.ToString();
            }
            _tossValue = (int)MathHelper.Clamp(_tossValue, 1, Core.Player.Inventory.GetItemAmount(itemID));

            if (TextBox.Showing == false)
            {
                if (Controls.Accept())
                {
                    SoundManager.PlaySound("select");
                    Core.Player.Inventory.RemoveItem(itemID, _tossValue);
                    LoadItems();
                    _tossingItems = false;
                }
                else if (Controls.Dismiss())
                {
                    SoundManager.PlaySound("select");
                    _tossingItems = false;
                }
                _tossValue = 1;
            }
        }
    }

    private void UpdateTabs()
    {
        if ((Controls.Left(true, true, true, true, true, true) && _tabInControl) || ControllerHandler.ButtonPressed(Buttons.LeftShoulder))
        {
            _tabIndex -= 1;
            if (_allowedPages.Length > 0 && _allowedPages.Contains(_tabIndex) == false)
            {
                while (_allowedPages.Contains(_tabIndex) == false)
                {
                    _tabIndex -= 1;
                    if (_tabIndex < 0) _tabIndex = 7;
                    else if (_tabIndex > 7) _tabIndex = 0;
                }
            }
            if (_tabIndex < 0) _tabIndex = 7;
            else if (_tabIndex > 7) _tabIndex = 0;
            _itemIntro = 0f;
            ResetAnimation();
            LoadItems();
        }
        if ((Controls.Right(true, true, true, true, true, true) && _tabInControl) || ControllerHandler.ButtonPressed(Buttons.RightShoulder))
        {
            _tabIndex += 1;
            if (_allowedPages.Length > 0 && _allowedPages.Contains(_tabIndex) == false)
            {
                while (_allowedPages.Contains(_tabIndex) == false)
                {
                    _tabIndex += 1;
                    if (_tabIndex < 0) _tabIndex = 7;
                    else if (_tabIndex > 7) _tabIndex = 0;
                }
            }
            if (_tabIndex < 0) _tabIndex = 7;
            else if (_tabIndex > 7) _tabIndex = 0;
            _itemIntro = 0f;
            ResetAnimation();
            LoadItems();
        }
        if (_tabInControl)
        {
            if (_allowedPages.Length == 1)
            {
                _tabInControl = false;
            }
            else
            {
                if (Controls.Dismiss() && CanExit)
                {
                    SoundManager.PlaySound("select");
                    SelectedItem = String.Empty;
                    _closing = true;
                }
                if (Controls.Accept() && _items.Length > 0)
                {
                    SoundManager.PlaySound("select");
                    _tabInControl = false;
                }
            }
        }
    }

    private void UpdateItems()
    {
        if (Controls.Left(true, true, false, true, true, true))
        {
            ItemIndex -= 2;
            if (ItemIndex < 0 && PageIndex > 0)
            {
                ItemIndex += 10;
                PageIndex -= 1;
                _itemIntro = 0f;
                ResetAnimation();
            }
            else if (ItemIndex < 0 && PageIndex == 0)
            {
                ItemIndex = (ItemIndex == -1) ? 1 : 0;
            }
        }
        if (Controls.Right(true, true, false, true, true, true))
        {
            if (ItemIndex + 2 + (PageIndex * 10) < _items.Length)
            {
                ItemIndex += 2;
                if (ItemIndex > 9)
                {
                    ItemIndex -= 10;
                    PageIndex += 1;
                    _itemIntro = 0f;
                    ResetAnimation();
                }
            }
        }
        if (Controls.Up(true, true, true, true, true, true))
        {
            ItemIndex -= 1;
            if (ItemIndex < 0 && PageIndex > 0)
            {
                ItemIndex += 10;
                PageIndex -= 1;
                _itemIntro = 0f;
                ResetAnimation();
            }
            else if (ItemIndex < 0 && PageIndex == 0)
            {
                ItemIndex = 0;
            }
        }
        if (Controls.Down(true, true, true, true, true, true))
        {
            if (ItemIndex + 1 + (PageIndex * 10) < _items.Length)
            {
                ItemIndex += 1;
                if (ItemIndex > 9)
                {
                    ItemIndex -= 10;
                    PageIndex += 1;
                    _itemIntro = 0f;
                    ResetAnimation();
                }
            }
        }

        if (Controls.Accept() && _items.Length > 0 && _items.Length - 1 >= ItemIndex + PageIndex * 10)
        {
            Items.Item cItem = Items.Item.GetItemByID(_items[ItemIndex + PageIndex * 10].ItemID);
            SoundManager.PlaySound("select");
            if (_doReturnItem == true)
            {
                if (cItem.IsGameModeItem == true)
                {
                    ReturnSelectedItem(cItem.gmID);
                }
                else
                {
                    ReturnSelectedItem(cItem.ID.ToString());
                }
            }
            else
            {
                if (PreScreen.Identification == Screen.Identifications.BattleScreen)
                {
                    if (cItem.CanBeUsedInBattle == true)
                    {
                        _infoItemOptionSelection = 0;
                        _isInfoShowing = true;
                        SetInfoSettings();
                        SetItemOptions();
                    }
                    else
                    {
                        TextBox.Show(Localization.GetString("inventory_screen_ItemNotUsableInBattle", "This item can't~be used in Battle."));
                    }
                }
                else
                {
                    _infoItemOptionSelection = 0;
                    _isInfoShowing = true;
                    SetInfoSettings();
                    SetItemOptions();
                }
            }
        }
        if (Controls.Dismiss())
        {
            if (_allowedPages.Length > 1)
            {
                SoundManager.PlaySound("select");
                _tabInControl = true;
            }
            else
            {
                _closing = true;
            }
        }
    }

    private void UpdateInfo()
    {
        for (int i = 0; i <= _infoItemOptionSize.Length - 1; i++)
        {
            if (i == _infoItemOptionSelection)
            {
                if (_infoItemOptionSize[i] < 200)
                {
                    _infoItemOptionSize[i] += 20;
                    if (_infoItemOptionSize[i] >= 200)
                    {
                        _infoItemOptionSize[i] = 200;
                    }
                }
            }
            else
            {
                if (_infoItemOptionSize[i] > 0)
                {
                    _infoItemOptionSize[i] -= 20;
                    if (_infoItemOptionSize[i] <= 0)
                    {
                        _infoItemOptionSize[i] = 0;
                    }
                }
            }
        }

        if (Controls.Up(true))
        {
            _infoItemOptionSelection -= 1;
            if (_infoItemOptionSelection < 0)
            {
                _infoItemOptionSelection = _infoItemOptions.Count - 1;
            }
        }
        if (Controls.Down(true))
        {
            _infoItemOptionSelection += 1;
            if (_infoItemOptionSelection > _infoItemOptions.Count - 1)
            {
                _infoItemOptionSelection = 0;
            }
        }

        if (Controls.Accept())
        {
            SoundManager.PlaySound("select");
            SelectedItemOption();
        }

        if (Controls.Dismiss())
        {
            SoundManager.PlaySound("select");
            CloseInfoScreen();
        }
    }

    private void CloseInfoScreen()
    {
        _infoSizeTarget = 0;
        _infoPositionTarget = GetInfoTargetPositionRollback();
        _itemColumnRightOffsetTarget = 0;
        _itemColumnLeftOffsetTarget = 0;
        _isInfoShowing = false;
    }

    private void SaveBagIndex()
    {
        Player.Temp.BagIndex = _tabIndex;
        Player.Temp.BagPageIndex = _pageIndex;
        Player.Temp.BagItemIndex = _itemindex;
    }

    private void SelectedItemOption()
    {
        if (_infoItemOptionsNormal.Count > 0)
        {
            Items.Item cItem = Items.Item.GetItemByID(_items[ItemIndex + PageIndex * 10].ItemID);

            switch (_infoItemOptionsNormal[_infoItemOptionSelection])
            {
                case INFO_ITEM_OPTION_USE:
                    cItem.Use();
                    LoadItems();
                    if (PreScreen.Identification == Screen.Identifications.BattleScreen)
                    {
                        if (cItem.IsGameModeItem == true)
                        {
                            Core.Player.UsedItemsToCheckScriptDelayFor.Add(cItem.gmID);
                        }
                        else
                        {
                            Core.Player.UsedItemsToCheckScriptDelayFor.Add(cItem.ID.ToString());
                        }
                    }
                    else
                    {
                        if (cItem.IsGameModeItem == true)
                        {
                            Core.Player.CheckItemCountScriptDelay(cItem.gmID);
                        }
                        else
                        {
                            Core.Player.CheckItemCountScriptDelay(cItem.ID.ToString());
                        }
                    }
                    break;

                case INFO_ITEM_OPTION_GIVE:
                    PartyScreen selScreen = new PartyScreen(Core.CurrentScreen, cItem, (PartyScreen.DoStuff?)null, Localization.GetString("global_give", "Give") + " " + cItem.OneLineName(), true, true, true, null, false)
                    {
                        Mode = ISelectionScreen.ScreenMode.Selection,
                        CanExit = true,
                        SelectButtonText = Localization.GetString("global_give", "Give")
                    };
                    selScreen.SelectedObject += GiveItemHandler;
                    Core.SetScreen(selScreen);
                    break;

                case INFO_ITEM_OPTION_TOSS:
                    TossItem(cItem);
                    break;

                case INFO_ITEM_OPTION_SELECT:
                    if (cItem.IsGameModeItem == true)
                    {
                        FireSelectionEvent(cItem.gmID);
                    }
                    else
                    {
                        FireSelectionEvent(cItem.ID.ToString());
                    }
                    CloseInfoScreen();
                    _closing = true;
                    break;
            }
            SaveBagIndex();
        }
    }

    private void TossItem(Items.Item cItem)
    {
        String text = Localization.GetString("inventory_screen_TossConfirmation", "Are you sure you want to toss~this item?") + "%" + Localization.GetString("global_yes", "Yes") + "|" + Localization.GetString("global_no", "No") + "%";
        TextBox.Show(text, TossManyItems, false, false, TextBox.DefaultColor);
    }

    private void TossManyItems(int result)
    {
        if (result == 0)
        {
            TextBox.Show(Localization.GetString("inventory_screen_TossAmount", "Select the amount to toss."), []);
            _tossingItems = true;
        }
    }

    private void GiveItemHandler(Object[] args)
    {
        GiveItem((int)args[0]);
    }

    private void GiveItem(int pokeIndex)
    {
        Items.Item cItem = Items.Item.GetItemByID(_items[ItemIndex + PageIndex * 10].ItemID);
        Pokemon pokemon = Core.Player.Pokemons[pokeIndex];

        if (Screen.Level.IsBugCatchingContest == true && pokemon.catchBall.ID == 177 && pokeIndex == 1)
        {
            ShowMessage(Localization.GetString("inventory_screen_CannotGiveBugContest", "Cannot give an item to~this Pokémon during a~Bug-Catching Contest."));
        }
        else
        {
            if (pokemon.IsEgg == false)
            {
                String itemID;
                if (cItem.IsGameModeItem == true)
                {
                    itemID = cItem.gmID;
                }
                else
                {
                    itemID = cItem.ID.ToString();
                }

                Core.Player.Inventory.RemoveItem(itemID, 1);

                Items.Item? reItem = null;
                if (pokemon.Item != null)
                {
                    String reItemID;
                    if (pokemon.Item.IsGameModeItem == true)
                    {
                        reItemID = ((Items.GameModeItem)pokemon.Item).gmID;
                    }
                    else
                    {
                        reItemID = pokemon.Item.ID.ToString();
                    }
                    reItem = pokemon.Item;
                    Core.Player.Inventory.AddItem(reItemID, 1);
                }

                pokemon.Item = Items.Item.GetItemByID(itemID);

                if (reItem == null)
                {
                    String giveString = Localization.GetString("inventory_screen_GiveItem_Give", "Gave <name> the <newitem>.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()).Replace("[NEWITEM]", cItem.OneLineName());
                    ShowMessage(giveString);
                }
                else
                {
                    String switchString = Localization.GetString("inventory_screen_GiveItem_Switch", "Switched <name>'s <olditem> with the <newitem>.").Replace("[POKEMONNAME]", pokemon.GetDisplayName()).Replace("[OLDITEM]", reItem.OneLineName()).Replace("[NEWITEM]", cItem.OneLineName());
                    ShowMessage(switchString);
                }

                LoadItems();
                if (ItemIndex + PageIndex * 10 > _items.Length - 1)
                {
                    ItemIndex = 0;
                    PageIndex = 0;
                    CloseInfoScreen();
                }
            }
            else
            {
                ShowMessage(Localization.GetString("inventory_screen_EggsCannotHold", "Eggs cannot hold items."));
            }
        }
    }

    private void ShowMessage(String text)
    {
        _messageDelay = (float)(text.Length / 1.75);
        _messageText = text;
    }

    private void ResetAnimation()
    {
        _itemAnimation = new ItemAnimation();
    }

    private void UpdateShakeAnimation()
    {
        if (_itemAnimation._shakeLeft == true)
        {
            _itemAnimation._shakeV -= 0.0275f;
            if (_itemAnimation._shakeV <= -0.4f)
            {
                _itemAnimation._shakeCount -= 1;
                _itemAnimation._shakeLeft = false;
            }
        }
        else
        {
            _itemAnimation._shakeV += 0.0275f;
            if (_itemAnimation._shakeV >= 0.4f)
            {
                _itemAnimation._shakeCount -= 1;
                _itemAnimation._shakeLeft = true;
            }
        }
    }

    public void LoadItems()
    {
        _items = Core.Player.Inventory
            .Where(x => Items.Item.GetItemByID(x.ItemID).ItemType == _visibleItemTypes[_tabIndex])
            .Where(x => IsItemVisible(x.ItemID) == true)
            .ToArray();

        if (_tabIndex == 4)
        {
            _items = _items.OrderBy(i => Items.Item.GetItemByID(i.ItemID).SortValue).ToArray();
        }
        else
        {
            _items = _items.OrderBy(i => Items.Item.GetItemByID(i.ItemID).Name).ToArray();
        }

        if (_items.Length <= ItemIndex + PageIndex * 10)
        {
            ItemIndex -= 1;
            if (ItemIndex == -1)
            {
                if (PageIndex > 0)
                {
                    PageIndex -= 1;
                    ItemIndex = 9;
                }
                else
                {
                    ItemIndex = 0;
                    PageIndex = 0;
                    _tabInControl = true;
                }
            }
            CloseInfoScreen();
        }
    }

    private void SetInfoSettings()
    {
        int column = (int)Math.Floor(ItemIndex / 2.0);

        _infoSize = 0;
        _infoSizeTarget = 500;
        _itemColumnLeftOffset = 0;
        _itemColumnRightOffset = 0;

        switch (column)
        {
            case 0:
                _infoSide = 0;
                _infoPosition = column * 160 + 32 + 48;
                _infoPositionTarget = _infoPosition;
                _itemColumnLeft = 0;
                _itemColumnLeftOffsetTarget = 0;
                _itemColumnRightOffsetTarget = 500;
                break;
            case 1:
                _infoSide = 0;
                _infoPosition = column * 160 + 32 + 48;
                _infoPositionTarget = _infoPosition - 160;
                _itemColumnLeft = 1;
                _itemColumnLeftOffsetTarget = -160;
                _itemColumnRightOffsetTarget = 340;
                break;
            case 2:
                _infoSide = 0;
                _infoPosition = column * 160 + 32 + 48;
                _infoPositionTarget = _infoPosition - 320;
                _itemColumnLeft = 2;
                _itemColumnLeftOffsetTarget = -320;
                _itemColumnRightOffsetTarget = 180;
                break;
            case 3:
                _infoSide = 1;
                _infoPosition = column * 160 - 80;
                _infoPositionTarget = _infoPosition - 320;
                _itemColumnLeft = 2;
                _itemColumnLeftOffsetTarget = -320;
                _itemColumnRightOffsetTarget = 180;
                break;
            case 4:
                _infoSide = 1;
                _infoPosition = column * 160 - 80;
                _infoPositionTarget = _infoPosition - 500;
                _itemColumnLeft = 3;
                _itemColumnLeftOffsetTarget = -500;
                _itemColumnRightOffsetTarget = 0;
                break;
        }
    }

    private void SetItemOptions()
    {
        _infoItemOptions.Clear();
        _infoItemOptionsNormal.Clear();

        Items.Item cItem = Items.Item.GetItemByID(_items[ItemIndex + PageIndex * 10].ItemID);

        if (_mode == ISelectionScreen.ScreenMode.Default)
        {
            if (cItem.CanBeUsed)
            {
                _infoItemOptions.Add(Localization.GetString("global_use", "Use"));
                _infoItemOptionsNormal.Add(INFO_ITEM_OPTION_USE);
            }
            if (cItem.CanBeHeld)
            {
                _infoItemOptions.Add(Localization.GetString("global_give", "Give"));
                _infoItemOptionsNormal.Add(INFO_ITEM_OPTION_GIVE);
            }
            if (cItem.CanBeTossed)
            {
                _infoItemOptions.Add(Localization.GetString("global_toss", "Toss"));
                _infoItemOptionsNormal.Add(INFO_ITEM_OPTION_TOSS);
            }
        }
        else if (_mode == ISelectionScreen.ScreenMode.Selection)
        {
            _infoItemOptions.Add(Localization.GetString("global_select", "Select"));
            _infoItemOptionsNormal.Add(INFO_ITEM_OPTION_SELECT);
        }
    }

    private int GetInfoTargetPositionRollback()
    {
        int column = (int)Math.Floor(ItemIndex / 2.0);

        return column switch
        {
            0 => column * 160 + 32 + 48,
            1 => column * 160 + 32 + 48,
            2 => column * 160 + 32 + 48,
            3 => column * 160 - 80,
            4 => column * 160 - 80,
            _ => 0
        };
    }

    private void UpdateInfoAnimation()
    {
        int tempInfoSize = _infoSize;
        _infoSize = (int)MathHelper.Lerp(_infoSize, _infoSizeTarget, 0.1f);
        if (tempInfoSize == _infoSize)
        {
            _infoSize = _infoSizeTarget;
        }

        int tempInfoPosition = _infoPosition;
        _infoPosition = (int)MathHelper.Lerp(_infoPosition, _infoPositionTarget, 0.1f);
        if (tempInfoPosition == _infoPosition)
        {
            _infoPosition = _infoPositionTarget;
        }

        int tempItemColumnLeftOffset = _itemColumnLeftOffset;
        _itemColumnLeftOffset = (int)MathHelper.Lerp(_itemColumnLeftOffset, _itemColumnLeftOffsetTarget, 0.1f);
        if (tempItemColumnLeftOffset == _itemColumnLeftOffset)
        {
            _itemColumnLeftOffset = _itemColumnLeftOffsetTarget;
        }

        int tempItemColumnRightOffset = _itemColumnRightOffset;
        _itemColumnRightOffset = (int)MathHelper.Lerp(_itemColumnRightOffset, _itemColumnRightOffsetTarget, 0.1f);
        if (tempItemColumnRightOffset == _itemColumnRightOffset)
        {
            _itemColumnRightOffset = _itemColumnRightOffsetTarget;
        }
    }

    private bool IsItemVisible(String itemID)
    {
        if (_allowedItems == null || _allowedItems.Contains("-1"))
        {
            return true;
        }
        return _allowedItems.Contains(itemID);
    }

    private void FireSelectionEvent(String itemId)
    {
        SelectedObject?.Invoke(new Object[] { itemId });
    }
}
