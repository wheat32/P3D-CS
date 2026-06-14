using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;
using P3D.Items;
using P3D.Screens.UI;

namespace P3D;

public class MailSystemScreen : Screen
{
    private int _index = -1;
    private int _scrollIndex = 0;
    private int _selectIndex = 0;

    private List<MenuEntry> _menuEntries = [];
    private bool _menuVisible = false;
    private int _menuCursor = 0;
    private String _menuHeader = String.Empty;
    private String _message = String.Empty;
    private bool _usedFromInventory = false;
    private bool _takenFromParty = false;
    private bool _readyToExit = false;

    private Items.MailItem.MailData _tempNewMail = new Items.MailItem.MailData();
    private int _editMailIndex = 0;

    public MailSystemScreen(Screen currentScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.MailSystemScreen;
        MouseVisible = true;
        CanBePaused = true;
        CanMuteAudio = false;
        CanChat = false;
    }

    public MailSystemScreen(Screen currentScreen, String inventoryMailItemID)
    {
        PreScreen = currentScreen;
        Identification = Identifications.MailSystemScreen;
        MouseVisible = true;
        CanBePaused = true;
        CanMuteAudio = false;
        CanChat = false;
        if (inventoryMailItemID != String.Empty)
        {
            _usedFromInventory = true;
            _index = 0;
            ChosenMail(inventoryMailItemID);
        }
    }

    public MailSystemScreen(Screen currentScreen, Items.MailItem partyMailItem)
    {
        PreScreen = currentScreen;
        Identification = Identifications.MailSystemScreen;
        MouseVisible = true;
        CanBePaused = true;
        CanMuteAudio = false;
        CanChat = false;
        if (partyMailItem != null)
        {
            _takenFromParty = true;
            _index = 1;
            _tempNewMail = Items.MailItem.GetMailDataFromString(partyMailItem.AdditionalData);
            _tempNewMail.MailRead = true;
        }
    }

    public MailSystemScreen(Screen currentScreen, Items.GameModeItem gmMailItem)
    {
        PreScreen = currentScreen;
        Identification = Identifications.MailSystemScreen;
        MouseVisible = true;
        CanBePaused = true;
        CanMuteAudio = false;
        CanChat = false;
        if (gmMailItem != null)
        {
            _takenFromParty = true;
            _index = 1;
            _tempNewMail = Items.MailItem.GetMailDataFromString(gmMailItem.AdditionalData);
            _tempNewMail.MailRead = true;
        }
    }

    public override void Draw()
    {
        Texture2D background = TextureManager.GetTexture("GUI\\Menus\\MailboxBackground");
        Size backSize = new Size(Core.windowSize.Width, Core.windowSize.Height);
        Size origSize = new Size(background.Width, background.Height);
        float aspectRatio = (float)(origSize.Width / origSize.Height);

        backSize.Width = (int)(Core.windowSize.Width * aspectRatio);
        backSize.Height = (int)(backSize.Width / aspectRatio);

        if (backSize.Width > backSize.Height)
        {
            backSize.Width = Core.windowSize.Width;
            backSize.Height = (int)(Core.windowSize.Width / aspectRatio);
        }
        else
        {
            backSize.Height = Core.windowSize.Height;
            backSize.Width = (int)(Core.windowSize.Height / aspectRatio);
        }
        if (backSize.Height < Core.windowSize.Height)
        {
            backSize.Height = Core.windowSize.Height;
            backSize.Width = (int)((float)Core.windowSize.Height / origSize.Height * origSize.Width);
        }

        int xOffset = 0;
        if (Core.windowSize.Width < backSize.Width)
        {
            float xAspectRatio = (float)(origSize.Width / backSize.Width);
            xOffset = (int)(Math.Floor((backSize.Width - Core.windowSize.Width) * xAspectRatio) / 2);
        }

        Core.SpriteBatch.Draw(background, new Rectangle(0, 0, backSize.Width, backSize.Height), new Rectangle(xOffset, 0, origSize.Width, origSize.Height), Color.White);
        Canvas.DrawRectangle(new Rectangle(32, 16, 240, 48), new Color(255, 255, 255, 224));
        Canvas.DrawRectangle(new Rectangle(48, 62, 208, 2), Color.DarkGray);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("mail_screen_title", "Mailbox"), new Vector2(56, 24), Color.Black);
        Canvas.DrawRectangle(new Rectangle(32, 64, 352, 624), new Color(255, 255, 255, 224));
        Canvas.DrawRectangle(new Rectangle(400, 64, 704, 624), new Color(255, 255, 255, 224));

        if (_usedFromInventory == true)
        {
            DrawMail(new Items.MailItem.MailData(), new Vector2(42, 78), 0);
        }
        else if (_takenFromParty == true)
        {
            DrawMail(_tempNewMail, new Vector2(42, 78), 1);
        }
        else
        {
            for (int i = _scrollIndex; i <= _scrollIndex + 8; i++)
            {
                if (i == 0)
                {
                    DrawMail(new Items.MailItem.MailData(), new Vector2(42, 78 + (i - _scrollIndex) * 64), i);
                }
                else
                {
                    if (i <= Core.Player.Mails.Count)
                        DrawMail(Core.Player.Mails[i - 1], new Vector2(42, 78 + (i - _scrollIndex) * 64 + 2 * (i - _scrollIndex)), i);
                }
            }
        }

        if (_usedFromInventory == false && _takenFromParty == false)
            Canvas.DrawScrollBar(new Vector2(368, 86), Core.Player.Mails.Count + 1, 9, _scrollIndex, new Size(6, 560), false, Color.LightGray, Color.Black);

        if (_index != -1)
            DrawCurrentMail();

        if (_menuVisible == true)
            DrawMenuEntries();

        if (_message != String.Empty)
        {
            Canvas.DrawRectangle(Core.windowSize, new Color(0, 0, 0, 150));
            String t = _message.CropStringToWidth(FontManager.MainFont, 800);
            Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2((float)(Core.windowSize.Width / 2 - FontManager.MainFont.MeasureString(t).X / 2), 340), Color.White);
        }
    }

    private void DrawMail(Items.MailItem.MailData mail, Vector2 p, int i)
    {
        if (i == 0)
        {
            int x = 16, y = 16;
            if (i == _index) { x = 80; y = 72; }
            else if (i == _selectIndex) { x = 48; y = 72; }

            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(x, y, 16, 16), String.Empty), new Rectangle((int)p.X, (int)p.Y, 64, 64), Color.White);
            for (int ix = 64; ix <= 224; ix += 64)
                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(x + 16, y, 16, 16), String.Empty), new Rectangle((int)p.X + ix, (int)p.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(x, y, 16, 16), String.Empty), new Rectangle((int)p.X + 256, (int)p.Y, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("mail_screen_write_new_mail", "Write new mail."), new Vector2((int)p.X + 13, (int)p.Y + 18), Color.Black);
        }
        else
        {
            Item item = Item.GetItemByID(mail.MailID.ToString())!;
            int x = 16, y = 16;
            if (i == _index) { x = 80; y = 72; }
            else if (i == _selectIndex) { x = 48; y = 72; }

            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(x, y, 16, 16), String.Empty), new Rectangle((int)p.X, (int)p.Y, 64, 64), Color.White);
            for (int ix = 64; ix <= 224; ix += 64)
                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(x + 16, y, 16, 16), String.Empty), new Rectangle((int)p.X + ix, (int)p.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(x, y, 16, 16), String.Empty), new Rectangle((int)p.X + 256, (int)p.Y, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

            Core.SpriteBatch.Draw(item.Texture, new Rectangle((int)p.X, (int)p.Y + 4, 48, 48), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, mail.MailHeader, new Vector2((int)p.X + 52, (int)p.Y + 18), Color.Black);

            if (mail.MailAttachment > -1)
            {
                TrophyInformation t = GetTrophyInformation(mail.MailAttachment);
                Core.SpriteBatch.Draw(t.Texture, new Rectangle((int)p.X + 250, (int)p.Y + 8, 32, 32), Color.White);
            }

            if (mail.MailRead == false)
                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\GTS"), new Rectangle((int)p.X + 272, (int)p.Y + 4, 32, 32), new Rectangle(320, 144, 32, 32), Color.White);
        }
    }

    private void DrawCurrentMail()
    {
        if (_index == 0)
        {
            Items.MailItem.MailData mail = _tempNewMail;
            Item item = Item.GetItemByID(mail.MailID.ToString())!;
            Core.SpriteBatch.Draw(item.Texture, new Rectangle(420, 84, 48, 48), Color.White);

            Color c = _editMailIndex == 0 ? Color.Blue : Color.Gray;
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("mail_screen_mail_header", "Header:") + " " + mail.MailHeader, new Vector2(480, 92), c);
            Canvas.DrawRectangle(new Rectangle(420, 140, 660, 2), Color.DarkGray);

            String text = (Localization.GetString("mail_screen_mail_text", "Text:") + " (" + mail.MailText.Length + "/" + 200 + ")" + Environment.NewLine + Environment.NewLine + mail.MailText.Replace("<br>", Environment.NewLine)).CropStringToWidth(FontManager.MainFont, 600);
            c = _editMailIndex == 1 ? Color.Blue : Color.Gray;
            if (_editMailIndex == 1) text += "_";
            Core.SpriteBatch.DrawString(FontManager.MainFont, text, new Vector2(430, 160), c);

            int yPlus = (int)FontManager.MainFont.MeasureString(text).Y;

            c = _editMailIndex == 2 ? Color.Blue : Color.Gray;
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("mail_screen_mail_signature", "Signature:") + " " + mail.MailSignature, new Vector2(430, yPlus + 200), c);
            Canvas.DrawRectangle(new Rectangle(420, yPlus + 240, 660, 2), Color.DarkGray);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("mail_screen_mail_ot", "Mail OT:") + " " + mail.MailSender + " (" + mail.MailOriginalTrainerOT + ")", new Vector2(430, yPlus + 260), Color.Black);

            DrawButton(440, yPlus + 320, Localization.GetString("mail_screen_mail_button_attach", "Attach"), 534, yPlus, _editMailIndex == 3);
            DrawButton(640, yPlus + 320, Localization.GetString("global_cancel", "Cancel"), 734, yPlus, _editMailIndex == 4);
        }
        else
        {
            Items.MailItem.MailData mail = _takenFromParty == true ? _tempNewMail : Core.Player.Mails[_index - 1];
            Item item = Item.GetItemByID(mail.MailID.ToString())!;
            Core.SpriteBatch.Draw(item.Texture, new Rectangle(420, 84, 48, 48), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, mail.MailHeader, new Vector2(480, 92), Color.Black);
            Canvas.DrawRectangle(new Rectangle(420, 140, 660, 2), Color.DarkGray);

            String text = mail.MailText.CropStringToWidth(FontManager.MainFont, 600);
            Core.SpriteBatch.DrawString(FontManager.MainFont, text, new Vector2(430, 160), Color.Black);

            int yPlus = (int)FontManager.MainFont.MeasureString(text).Y;

            Core.SpriteBatch.DrawString(FontManager.MainFont, mail.MailSignature, new Vector2(430, yPlus + 200), Color.Black);
            Canvas.DrawRectangle(new Rectangle(420, yPlus + 240, 660, 2), Color.DarkGray);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("mail_screen_mail_ot", "Mail OT:") + " " + mail.MailSender + " (" + mail.MailOriginalTrainerOT + ")", new Vector2(430, yPlus + 260), Color.Black);

            if (mail.MailAttachment > -1)
            {
                Canvas.DrawRectangle(new Rectangle(420, yPlus + 300, 660, 2), Color.DarkGray);
                TrophyInformation t = GetTrophyInformation(mail.MailAttachment);
                Core.SpriteBatch.DrawString(FontManager.MainFont, "Trophy:", new Vector2(430, yPlus + 320), Color.Black);
                Core.SpriteBatch.Draw(t.Texture, new Rectangle(430, yPlus + 340, 64, 64), Color.White);
                Core.SpriteBatch.DrawString(FontManager.MainFont, (t.Name + Environment.NewLine + Environment.NewLine + t.Description).CropStringToWidth(FontManager.MainFont, 500), new Vector2(510, yPlus + 340), Color.Black);
            }

            DrawButton(440, yPlus + 320, Localization.GetString("mail_screen_mail_button_attach", "Attach"), 534, yPlus, _editMailIndex == 0);
            String btn2 = _takenFromParty == true
                ? Localization.GetString("mail_screen_mail_button_send_to_pc", "Send To PC")
                : Localization.GetString("global_delete", "Delete");
            DrawButton(640, yPlus + 320, btn2, 734, yPlus, _editMailIndex == 1);

            if (_takenFromParty == false)
                DrawButton(840, yPlus + 320, Localization.GetString("global_cancel", "Cancel"), 934, yPlus, _editMailIndex == 2);
        }
    }

    private void DrawButton(int x, int y, String label, int textCenterX, int yPlus, bool selected)
    {
        Rectangle r16sel = new Rectangle(80, 72, 16, 16);
        Rectangle r16mid = new Rectangle(96, 72, 16, 16);
        Rectangle r16nor = new Rectangle(16, 16, 16, 16);
        Rectangle r16nm = new Rectangle(32, 16, 16, 16);

        Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", selected ? r16sel : r16nor, String.Empty), new Rectangle(x, y, 64, 64), Color.White);
        Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", selected ? r16mid : r16nm, String.Empty), new Rectangle(x + 64, y, 64, 64), Color.White);
        Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\General", selected ? r16sel : r16nor, String.Empty), new Rectangle(x + 128, y, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
        Core.SpriteBatch.DrawString(FontManager.MainFont, label, new Vector2((int)(textCenterX - FontManager.MainFont.MeasureString(label).X / 2), yPlus + (int)(348 - FontManager.MainFont.MeasureString(label).Y / 2)), Color.Black);
    }

    public override void Update()
    {
        if (_message != String.Empty)
        {
            if (Controls.Accept(true, true, true) == true || Controls.Dismiss(true, true, true) == true)
            {
                _message = String.Empty;
                if (_readyToExit == true)
                    Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.Black, false));
            }
            return;
        }

        if (_menuVisible == true)
        {
            for (int i = 0; i <= _menuEntries.Count - 1; i++)
            {
                if (i <= _menuEntries.Count - 1)
                    _menuEntries[i].Update(this);
            }
            if (Controls.Up(true, true) == true) _menuCursor -= 1;
            if (Controls.Down(true, true) == true) _menuCursor += 1;

            int maxIndex = 0, minIndex = 100;
            foreach (MenuEntry e in _menuEntries)
            {
                if (e.Index < minIndex) minIndex = e.Index;
                if (e.Index > maxIndex) maxIndex = e.Index;
            }
            if (_menuCursor > maxIndex) _menuCursor = minIndex;
            else if (_menuCursor < minIndex) _menuCursor = maxIndex;
            return;
        }

        if (_index != 0)
        {
            if (_index > -1)
            {
                UpdateMailView();
            }
            else
            {
                UpdateMailList();
            }
        }
        else
        {
            UpdateNewMail();
        }
    }

    private void UpdateMailView()
    {
        if (Controls.Down(true, true, true, false, true) == true || (KeyBoardHandler.KeyPressed(Keys.Tab) == true && Controls.ShiftDown() == false))
            _editMailIndex += 1;
        if (Controls.Up(true, true, true, false, true) == true || (KeyBoardHandler.KeyPressed(Keys.Tab) == true && Controls.ShiftDown() == true))
            _editMailIndex -= 1;
        if (Controls.Left(true, true, false, true, true) == true) _editMailIndex -= 1;
        if (Controls.Right(true, true, false, true, true) == true) _editMailIndex += 1;

        _editMailIndex = _takenFromParty == true ? _editMailIndex.Clamp(0, 1) : _editMailIndex.Clamp(0, 2);

        if (Controls.Accept(false, true, true) == true)
        {
            switch (_editMailIndex)
            {
                case 0:
                    if (_takenFromParty == false)
                        _tempNewMail = Core.Player.Mails[_index - 1];
                    SoundManager.PlaySound("select");
                    PartyScreen selScreen = new PartyScreen(this, Item.GetItemByID(_tempNewMail.MailID.ToString())!, ChosenPokemon, Localization.GetString("mail_screen_give_mail_to", "Give mail to:"), true);
                    selScreen.Mode = ISelectionScreen.ScreenMode.Selection;
                    selScreen.CanExit = true;
                    selScreen.SelectedObject += ChosenPokemonHandler;
                    Core.SetScreen(selScreen);
                    break;
                case 1:
                    SoundManager.PlaySound("select");
                    if (_takenFromParty == true)
                        SetupMenu([new MenuEntry(3, Localization.GetString("global_yes", "Yes"), false, SendMailToPC), new MenuEntry(4, Localization.GetString("global_no", "No"), true, null)], Localization.GetString("mail_screen_mail_send_to_pc_confirm", "Send this mail to PC?"));
                    else
                        SetupMenu([new MenuEntry(3, Localization.GetString("global_yes", "Yes"), false, DeleteMail), new MenuEntry(4, Localization.GetString("global_no", "No"), true, null)], Localization.GetString("mail_screen_mail_delete_confirm", "Delete this mail?"));
                    break;
                case 2:
                    if (_takenFromParty == false) { _index = -1; _editMailIndex = 0; }
                    break;
            }
        }

        String text = String.Empty;
        if (_index != -1)
            text = (_takenFromParty == false ? Core.Player.Mails[_index - 1].MailText : _tempNewMail.MailText).CropStringToWidth(FontManager.MainFont, 600);
        int yPlus = (int)FontManager.MainFont.MeasureString(text).Y;

        if (Controls.Accept(true, false, false) == true)
        {
            int mailIndex = -1;
            if (_takenFromParty == false)
            {
                for (int i = 0; i <= 8; i++)
                {
                    if (i < Core.Player.Mails.Count + 1)
                    {
                        if (new Rectangle(46, 82 + 64 * i + 2 * (i - _scrollIndex), 288, 64).Contains(MouseHandler.MousePosition) == true)
                        {
                            mailIndex = _scrollIndex + i;
                            break;
                        }
                    }
                    else
                    {
                        if (new Rectangle(46, 82 + 64 * i + 2 * (i - _scrollIndex), 288, 64).Contains(MouseHandler.MousePosition) == true)
                        {
                            _editMailIndex = 1;
                            break;
                        }
                    }
                }
            }

            if (mailIndex != -1)
            {
                _selectIndex = mailIndex;
                if (_selectIndex == 0) { _index = -1; SoundManager.PlaySound("select"); OpenMailSelector(); }
                else
                {
                    SoundManager.PlaySound("select");
                    if (_index == _selectIndex) _index = -1;
                    else { _index = _selectIndex; MarkMailRead(_index); }
                }
            }
            else
            {
                if (new Rectangle(440, yPlus + 320, 192, 64).Contains(MouseHandler.MousePosition) == true)
                {
                    _editMailIndex = 0;
                    if (_takenFromParty == false) _tempNewMail = Core.Player.Mails[_index - 1];
                    SoundManager.PlaySound("select");
                    PartyScreen sel = new PartyScreen(this, Item.GetItemByID(_tempNewMail.MailID.ToString())!, ChosenPokemon, Localization.GetString("mail_screen_give_mail_to", "Give mail to:"), true);
                    sel.Mode = ISelectionScreen.ScreenMode.Selection;
                    sel.CanExit = true;
                    sel.SelectedObject += ChosenPokemonHandler;
                    Core.SetScreen(sel);
                }
                if (new Rectangle(640, yPlus + 320, 192, 64).Contains(MouseHandler.MousePosition) == true)
                {
                    SoundManager.PlaySound("select");
                    _editMailIndex = 1;
                    if (_takenFromParty == true)
                        SetupMenu([new MenuEntry(3, Localization.GetString("global_yes", "Yes"), false, SendMailToPC), new MenuEntry(4, Localization.GetString("global_no", "No"), true, null)], Localization.GetString("mail_screen_mail_send_to_pc_confirm", "Send this mail to PC?"));
                    else
                        SetupMenu([new MenuEntry(3, Localization.GetString("global_yes", "Yes"), false, DeleteMail), new MenuEntry(4, Localization.GetString("global_no", "No"), true, null)], Localization.GetString("mail_screen_mail_delete_confirm", "Delete this mail?"));
                }
                if (_takenFromParty == false && new Rectangle(840, yPlus + 320, 192, 64).Contains(MouseHandler.MousePosition) == true)
                {
                    SoundManager.PlaySound("select");
                    _index = -1;
                    _editMailIndex = 0;
                }
            }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            if (_takenFromParty == true)
                SetupMenu([new MenuEntry(3, Localization.GetString("global_yes", "Yes"), false, SendMailToPC), new MenuEntry(4, Localization.GetString("global_no", "No"), true, null)], Localization.GetString("mail_screen_mail_send_to_pc_confirm", "Send this mail to PC?"));
            else { SoundManager.PlaySound("select"); _index = -1; }
        }
    }

    private void UpdateMailList()
    {
        if (Controls.Down(true, true, true, true, true) == true)
        {
            _selectIndex += 1;
            if (_selectIndex == _index && _selectIndex < Core.Player.Mails.Count) _selectIndex += 1;
        }
        if (Controls.Up(true, true, true, true, true) == true)
        {
            _selectIndex -= 1;
            if (_selectIndex == _index && _selectIndex > 0) _selectIndex -= 1;
        }

        _selectIndex = _selectIndex.Clamp(0, Core.Player.Mails.Count);

        while (_selectIndex - _scrollIndex > 8) _scrollIndex += 1;
        while (_selectIndex - _scrollIndex < 0) _scrollIndex -= 1;

        if (Controls.Accept(false, true, true) == true)
        {
            if (_selectIndex == 0) { SoundManager.PlaySound("select"); OpenMailSelector(); }
            else
            {
                SoundManager.PlaySound("select");
                if (_index == _selectIndex) _index = -1;
                else { _index = _selectIndex; MarkMailRead(_index); }
            }
        }

        if (Controls.Accept(true, false, false) == true)
        {
            int mailIndex = -1;
            for (int i = 0; i <= 8; i++)
            {
                if (i < Core.Player.Mails.Count + 1)
                {
                    if (new Rectangle(46, 82 + 64 * i + 2 * (i - _scrollIndex), 288, 64).Contains(MouseHandler.MousePosition) == true)
                    {
                        mailIndex = _scrollIndex + i;
                        break;
                    }
                }
            }
            if (mailIndex != -1)
            {
                _selectIndex = mailIndex;
                if (_selectIndex == 0) { SoundManager.PlaySound("select"); OpenMailSelector(); }
                else
                {
                    SoundManager.PlaySound("select");
                    if (_index == _selectIndex) _index = -1;
                    else { _index = _selectIndex; MarkMailRead(_index); }
                }
            }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            SoundManager.PlaySound("select");
            Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.Black, false));
        }
    }

    private void UpdateNewMail()
    {
        bool pressedSystemKey = false;
        if (Controls.Down(true, true, true, false, true) == true || (KeyBoardHandler.KeyPressed(Keys.Tab) == true && Controls.ShiftDown() == false))
        { _editMailIndex += 1; pressedSystemKey = true; }
        if (Controls.Up(true, true, true, false, true) == true || (KeyBoardHandler.KeyPressed(Keys.Tab) == true && Controls.ShiftDown() == true))
        { _editMailIndex -= 1; pressedSystemKey = true; }
        if (Controls.Left(true, true, false, false, false) == true)
        { pressedSystemKey = true; if (_editMailIndex == 4 || _editMailIndex == 5) _editMailIndex -= 1; }
        if (Controls.Right(true, true, false, false, false) == true)
        { pressedSystemKey = true; if (_editMailIndex == 3 || _editMailIndex == 4) _editMailIndex += 1; }
        if (Controls.Left(false, false, false, true, true) == true)
        { if (_editMailIndex == 4 || _editMailIndex == 5) _editMailIndex -= 1; }
        if (Controls.Right(false, false, false, true, true) == true)
        { if (_editMailIndex == 3 || _editMailIndex == 4) _editMailIndex += 1; }

        _editMailIndex = _editMailIndex.Clamp(0, 4);

        if (pressedSystemKey == false)
        {
            switch (_editMailIndex)
            {
                case 0:
                {
                    String header = _tempNewMail.MailHeader;
                    KeyBindings.GetInput(ref header, 25, true, true);
                    _tempNewMail.MailHeader = header.Replace("\\,", ",").Replace(Environment.NewLine, "").Replace("|", "/");
                    break;
                }
                case 1:
                {
                    String mailText = _tempNewMail.MailText;
                    KeyBindings.GetInput(ref mailText, 200, true, true);
                    _tempNewMail.MailText = mailText.Replace("\\,", ",").Replace(Environment.NewLine, "<br>").Replace("|", "/");
                    break;
                }
                case 2:
                {
                    String sig = _tempNewMail.MailSignature;
                    KeyBindings.GetInput(ref sig, 25, true, true);
                    _tempNewMail.MailSignature = sig.Replace("\\,", ",").Replace(Environment.NewLine, "").Replace("|", "/");
                    break;
                }
            }
        }

        if (Controls.Accept(false, true, true) == true)
        {
            switch (_editMailIndex)
            {
                case 3:
                    SoundManager.PlaySound("select");
                    if (_tempNewMail.MailHeader == String.Empty || _tempNewMail.MailText == String.Empty || _tempNewMail.MailSignature == String.Empty)
                        _message = Localization.GetString("mail_screen_mail_missing_contents", "Please fill in the Header, the Message and the Signature.");
                    else
                    {
                        PartyScreen sel = new PartyScreen(this, Item.GetItemByID(_tempNewMail.MailID.ToString())!, ChosenPokemon, Localization.GetString("mail_screen_give_mail_to", "Give mail to:"), true);
                        sel.Mode = ISelectionScreen.ScreenMode.Selection;
                        sel.CanExit = true;
                        sel.SelectedObject += ChosenPokemonHandler;
                        Core.SetScreen(sel);
                    }
                    break;
                case 4:
                    SoundManager.PlaySound("select");
                    _index = -1;
                    _editMailIndex = 0;
                    break;
            }
        }

        String text = (Localization.GetString("mail_screen_mail_text", "Text:") + " (" + _tempNewMail.MailText.Length + "/" + 200 + ")" + Environment.NewLine + Environment.NewLine + _tempNewMail.MailText.Replace("<br>", Environment.NewLine)).CropStringToWidth(FontManager.MainFont, 600);
        int yPlus = (int)FontManager.MainFont.MeasureString(text).Y;

        if (Controls.Accept(true, false, false) == true)
        {
            int mailIndex = -1;
            if (_usedFromInventory == false)
            {
                for (int i = 0; i <= 8; i++)
                {
                    if (i < Core.Player.Mails.Count + 1)
                    {
                        if (new Rectangle(46, 82 + 64 * i + 2 * (i - _scrollIndex), 288, 64).Contains(MouseHandler.MousePosition) == true)
                        { mailIndex = _scrollIndex + i; break; }
                    }
                    else
                    {
                        if (new Rectangle(46, 82 + 64 * i + 2 * (i - _scrollIndex), 288, 64).Contains(MouseHandler.MousePosition) == true)
                        { _editMailIndex = 4; break; }
                    }
                }
            }
            if (mailIndex != -1)
            {
                _selectIndex = mailIndex;
                SoundManager.PlaySound("select");
                if (_selectIndex == 0) _index = -1;
                else { _index = _selectIndex; MarkMailRead(_index); }
            }

            if (new Rectangle(420, 92, 660, 40).Contains(MouseHandler.MousePosition) == true) _editMailIndex = 0;
            if (new Rectangle(420, 140, 660, yPlus + 20).Contains(MouseHandler.MousePosition) == true) _editMailIndex = 1;
            if (new Rectangle(420, yPlus + 200, 660, 40).Contains(MouseHandler.MousePosition) == true) _editMailIndex = 2;
            if (new Rectangle(440, yPlus + 320, 192, 64).Contains(MouseHandler.MousePosition) == true) _editMailIndex = 3;
            if (new Rectangle(640, yPlus + 320, 192, 64).Contains(MouseHandler.MousePosition) == true) _editMailIndex = 4;

            switch (_editMailIndex)
            {
                case 3:
                    SoundManager.PlaySound("select");
                    if (_tempNewMail.MailHeader == String.Empty || _tempNewMail.MailText == String.Empty || _tempNewMail.MailSignature == String.Empty)
                        _message = Localization.GetString("mail_screen_mail_missing_contents", "Please fill in the Header, the Message and the Signature.");
                    else
                    {
                        PartyScreen sel = new PartyScreen(this, Item.GetItemByID(_tempNewMail.MailID.ToString())!, ChosenPokemon, Localization.GetString("mail_screen_give_mail_to", "Give mail to:"), true);
                        sel.Mode = ISelectionScreen.ScreenMode.Selection;
                        sel.CanExit = true;
                        sel.SelectedObject += ChosenPokemonHandler;
                        Core.SetScreen(sel);
                    }
                    break;
                case 4:
                    SoundManager.PlaySound("select");
                    if (_usedFromInventory == true)
                        Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.Black, false));
                    else { _index = -1; _editMailIndex = 0; }
                    break;
            }
        }

        if (Controls.Dismiss(true, false, true) == true)
        {
            if (_usedFromInventory == true)
                Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.Black, false));
            else { _index = -1; _editMailIndex = 0; }
        }
    }

    private void MarkMailRead(int index)
    {
        Items.MailItem.MailData m = Core.Player.Mails[index - 1];
        Core.Player.Mails[index - 1] = new Items.MailItem.MailData
        {
            MailHeader = m.MailHeader, MailID = m.MailID, MailOriginalTrainerOT = m.MailOriginalTrainerOT,
            MailAttachment = m.MailAttachment, MailRead = true, MailSender = m.MailSender,
            MailSignature = m.MailSignature, MailText = m.MailText
        };
    }

    private void OpenMailSelector()
    {
        NewInventoryScreen selScreen = new NewInventoryScreen(Core.CurrentScreen, new int[] { 5 }, 5, null);
        selScreen.Mode = ISelectionScreen.ScreenMode.Selection;
        selScreen.CanExit = true;
        selScreen.SelectedObject += ChosenMailHandler;
        Core.SetScreen(selScreen);
    }

    private void ChosenMailHandler(Object[] parms)
    {
        ChosenMail((String)parms[0]);
    }

    private void ChosenMail(String itemID)
    {
        _index = 0;
        _editMailIndex = 0;
        _tempNewMail = new Items.MailItem.MailData
        {
            MailID = itemID,
            MailSender = Core.Player.Name,
            MailOriginalTrainerOT = Core.Player.OT,
            MailText = String.Empty,
            MailHeader = String.Empty,
            MailAttachment = -1,
            MailRead = false,
            MailSignature = String.Empty
        };
    }

    private void ChosenPokemonHandler(Object[] parms)
    {
        ChosenPokemon((int)parms[0]);
    }

    private void ChosenPokemon(int pokeIndex)
    {
        String text = Localization.GetString("mail_screen_mail_attached_mail", "Attached the Mail to [POKEMONNAME].").Replace("[POKEMONNAME]", Core.Player.Pokemons[pokeIndex].GetDisplayName());

        if (Core.Player.Pokemons[pokeIndex].Item != null)
        {
            Item heldItem = Core.Player.Pokemons[pokeIndex].Item!;
            if (heldItem.IsGameModeItem == true)
                Core.Player.Inventory.AddItem(heldItem.gmID, 1);
            else
                Core.Player.Inventory.AddItem(heldItem.ID.ToString(), 1);
            text = Localization.GetString("mail_screen_mail_taken_item_and_attached_mail", "Taken [ITEM] from [POKEMONNAME], and attached the Mail to [POKEMONNAME].")
                .Replace("[POKEMONNAME]", Core.Player.Pokemons[pokeIndex].GetDisplayName())
                .Replace("[ITEM]", heldItem.OneLineName());
        }

        Core.Player.Pokemons[pokeIndex].Item = Item.GetItemByID(_tempNewMail.MailID.ToString());
        Core.Player.Pokemons[pokeIndex].Item!.AdditionalData = Items.MailItem.GetStringFromMail(_tempNewMail);

        if (_index == 0)
        {
            Core.Player.Inventory.RemoveItem(_tempNewMail.MailID.ToString(), 1);
            _index = -1;
        }
        else
        {
            if (_takenFromParty == false)
            {
                Core.Player.Mails.RemoveAt(_index - 1);
                _selectIndex -= 1;
                _selectIndex = _selectIndex.Clamp(0, Core.Player.Mails.Count);
                _index = -1;
            }
            else
            {
                _readyToExit = true;
            }
        }

        Screen s = Core.CurrentScreen;
        while (s.PreScreen != null && s.Identification != Screen.Identifications.InventoryScreen)
            s = s.PreScreen;

        if (s.Identification == Screen.Identifications.InventoryScreen)
            ((NewInventoryScreen)s).LoadItems();

        _message = text;
    }

    private void DeleteMail()
    {
        _message = Localization.GetString("mail_screen_mail_deleted_from_mailbox", "The mail has been removed from the mailbox.");
        Core.Player.Mails.RemoveAt(_index - 1);
        _index = -1;
        _editMailIndex = 0;
        _selectIndex -= 1;
        _selectIndex = _selectIndex.Clamp(0, Core.Player.Mails.Count);
    }

    private void SendMailToPC()
    {
        _message = Localization.GetString("mail_screen_mail_added_to_mailbox", "The Mail was taken to your inbox on your PC.");
        Core.Player.Mails.Add(_tempNewMail);
        _readyToExit = true;
    }

    private void SetupMenu(MenuEntry[] entries, String header)
    {
        _menuEntries.Clear();
        _menuEntries.AddRange(entries);
        _menuVisible = true;
        _menuCursor = _menuEntries[0].Index;
        _menuHeader = header;
    }

    private void DrawMenuEntries()
    {
        if (_menuHeader != String.Empty)
        {
            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width - 14 - FontManager.MainFont.MeasureString(_menuHeader).X - 16), 100, (int)(FontManager.MainFont.MeasureString(_menuHeader).X + 16), 64), new Color(0, 0, 0, 180));
            Core.SpriteBatch.DrawString(FontManager.MainFont, _menuHeader, new Vector2(Core.windowSize.Width - 14 - FontManager.MainFont.MeasureString(_menuHeader).X - 8, 120), Color.White);
        }
        foreach (MenuEntry e in _menuEntries)
            e.Draw(_menuCursor, TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty));
    }

    public struct TrophyInformation
    {
        public String Name;
        public int ID;
        public Texture2D Texture;
        public String Description;
    }

    public static TrophyInformation GetTrophyInformation(int id)
    {
        TrophyInformation t = new TrophyInformation();
        Vector2 texturePosition = Vector2.Zero;

        switch (id)
        {
            case 0:
                t.Name = "Won a GTS competition.";
                t.Description = "You are the winner of a competition that took place at the GTS. It must have been an important competition.";
                texturePosition = new Vector2(0, 0);
                break;
            case 1:
                t.Name = "Won a GTS competition.";
                t.Description = "You are the winner of a competition that took place at the GTS.";
                texturePosition = new Vector2(32, 0);
                break;
            case 2:
                t.Name = "Won a GTS competition.";
                t.Description = "You are the winner of a competition that took place at the GTS.";
                texturePosition = new Vector2(64, 0);
                break;
            case 3:
                t.Name = "Kolben Support";
                t.Description = "This proves that the Kolben Support helped you with your game.";
                texturePosition = new Vector2(96, 0);
                break;
        }

        t.Texture = TextureManager.GetTexture("GUI\\Trophies", new Rectangle((int)texturePosition.X, (int)texturePosition.Y, 32, 32), String.Empty);
        t.ID = id;
        return t;
    }

    public class MenuEntry
    {
        public int Index = 0;
        public Object? TAG = null;
        public String Text = "Menu";
        public bool IsBack = false;
        public Action? ClickHandler = null;

        private Texture2D _t1 = null!;
        private Texture2D _t2 = null!;

        public MenuEntry(int index, String text, bool isBack, Action? clickHandler)
            : this(index, text, isBack, clickHandler, null) { }

        public MenuEntry(int index, String text, bool isBack, Action? clickHandler, Object? tag)
        {
            Index = index;
            TAG = tag;
            Text = text;
            IsBack = isBack;
            ClickHandler = clickHandler;
            _t1 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(16, 16, 16, 16), String.Empty);
            _t2 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(32, 16, 16, 16), String.Empty);
        }

        public void Update(MailSystemScreen s)
        {
            bool acceptMouse = Controls.Accept(true, false, false) == true && s._menuCursor == Index && new Rectangle(Core.windowSize.Width - 270, 66 * Index, 256, 64).Contains(MouseHandler.MousePosition) == true;
            bool acceptKey = Controls.Accept(false, true, true) == true && s._menuCursor == Index;
            bool dismiss = Controls.Dismiss(true, true, true) == true && IsBack == true;

            if (acceptMouse || acceptKey || dismiss)
            {
                s._menuVisible = false;
                ClickHandler?.Invoke();
            }
            if (new Rectangle(Core.windowSize.Width - 270, 66 * Index, 256, 64).Contains(MouseHandler.MousePosition) == true && Controls.Accept(true, false, false) == true)
                s._menuCursor = Index;
        }

        public void Draw(int cursorIndex, Texture2D cursorTexture)
        {
            Vector2 startPos = new Vector2(Core.windowSize.Width - 270, 66 * Index);
            Core.SpriteBatch.Draw(_t1, new Rectangle((int)startPos.X, (int)startPos.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(_t2, new Rectangle((int)startPos.X + 64, (int)startPos.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(_t2, new Rectangle((int)startPos.X + 128, (int)startPos.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(_t1, new Rectangle((int)startPos.X + 192, (int)startPos.Y, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Text, new Vector2(startPos.X + 128 - (FontManager.MainFont.MeasureString(Text).X * 1.4F) / 2, startPos.Y + 15), Color.Black, 0.0F, Vector2.Zero, 1.4F, SpriteEffects.None, 0.0F);

            if (Index == cursorIndex)
            {
                Vector2 cPos = new Vector2(startPos.X + 128, startPos.Y - 40);
                Core.SpriteBatch.Draw(cursorTexture, new Rectangle((int)cPos.X, (int)cPos.Y, 64, 64), Color.White);
            }
        }
    }
}
