using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;
using P3D.BattleSystem;
using P3D.Items;

namespace P3D;

public class PokemonScreen : Screen
{
    private int _index = 0;
    private Texture2D _mainTexture = null!;
    private float _yOffset = 0;
    private int _menuID = 0;
    private bool _mPressed = false;
    private int _switchIndex = -1;

    public PokemonScreen(Screen currentScreen, int pokeIndex)
    {
        Identification = Identifications.PokemonScreen;
        PreScreen = currentScreen;
        _mainTexture = TextureManager.GetTexture("GUI\\Menus\\Menu");
        _index = pokeIndex;

        if (Core.Player.Pokemons.Count == 6)
        {
            bool has100 = true;
            for (int i = 0; i <= 5; i++)
            {
                if (Core.Player.Pokemons[i].Level < 100)
                {
                    has100 = false;
                    break;
                }
            }
            if (has100 == true)
                GameJolt.Emblem.AchieveEmblem("overkill");
        }

        CheckForLegendaryEmblem();
    }

    public override void Draw()
    {
        PreScreen!.Draw();

        Texture2D canvasTexture = TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(60, 100, 800, 480));
        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(60, 100, 480, 64));
        Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("pokemon_screen_choose_a_pokemon"), new Vector2(142, 132), Color.Black);
        Core.SpriteBatch.Draw(_mainTexture, new Rectangle(78, 124, 48, 48), new Rectangle(96, 16, 18, 18), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("pokemon_screen_backadvice"), new Vector2(1200 - FontManager.MiniFont.MeasureString(Localization.GetString("pokemon_screen_backadvice")).X - 330, 580), Color.DarkGray);

        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
            DrawPokemonTile(i, Core.Player.Pokemons[i]);
        if (Core.Player.Pokemons.Count < 6)
        {
            for (int i = Core.Player.Pokemons.Count; i <= 5; i++)
                DrawEmptyTile(i);
        }

        if (ChooseBox.Showing == true)
        {
            Vector2 position = new Vector2(0, 0);
            switch (_index)
            {
                case 0: case 2: case 4: position = new Vector2(606, 566 - ChooseBox.Options.Length * 48); break;
                case 1: case 3: case 5: position = new Vector2(60, 566 - ChooseBox.Options.Length * 48); break;
            }
            ChooseBox.Draw(position, true, 1.0F);
        }

        TextBox.Draw();
    }

    public override void Update()
    {
        TextBox.reDelay = 0.0F;
        _yOffset += 0.45F;

        if (TextBox.Showing == true)
        {
            TextBox.Update();
        }
        else
        {
            MouseState mState = Mouse.GetState();
            if (_mPressed == false)
            {
                if (ChooseBox.Showing == false)
                    NavigateMain();
                else
                    ChooseBox.Update();

                if (Controls.Accept() == true)
                {
                    _mPressed = true;
                    AcceptKeyPressed();
                }
            }
            else
            {
                if (mState.LeftButton == ButtonState.Released && MouseHandler.ButtonUp(MouseHandler.MouseButtons.LeftButton) == true)
                    _mPressed = false;
            }

            if (Controls.Dismiss() == true)
                CancelKeyPressed();
        }
    }

    private void AcceptKeyPressed()
    {
        if (ChooseBox.Showing == true)
        {
            switch (_menuID)
            {
                case 0:
                    String chosen = ChooseBox.Options[ChooseBox.result];
                    if (chosen == Localization.GetString("pokemon_screen_summary"))
                    {
                        ChooseBox.Showing = false;
                        Core.SetScreen(new PokemonStatusScreen(this, _index, [], Core.Player.Pokemons[_index], true));
                    }
                    else if (chosen == Localization.GetString("pokemon_screen_switch"))
                    {
                        _switchIndex = _index;
                        ChooseBox.Showing = false;
                    }
                    else if (chosen == Localization.GetString("pokemon_screen_item"))
                    {
                        ChooseBox.Show([Localization.GetString("pokemon_screen_item_give"), Localization.GetString("pokemon_screen_item_take"), Localization.GetString("pokemon_screen_item_back")], 0, []);
                        _menuID = 1;
                    }
                    else if (chosen == Localization.GetString("pokemon_screen_back"))
                    {
                        ChooseBox.Showing = false;
                    }
                    else if (chosen == "Flash") { UseFlash(); }
                    else if (chosen == "Fly") { UseFly(); }
                    else if (chosen == "Ride") { UseRide(); }
                    else if (chosen == "Cut") { UseCut(); }
                    else if (chosen == "Dig") { UseDig(); }
                    else if (chosen == "Teleport") { UseTeleport(); }
                    break;
                case 1:
                    switch (ChooseBox.result)
                    {
                        case 0:
                            Core.SetScreen(new NewInventoryScreen(this, [], GiveItem));
                            break;
                        case 1:
                            TakeItem();
                            break;
                        case 2:
                            ShowMenu();
                            break;
                    }
                    break;
            }
        }
        else
        {
            if (_switchIndex == -1)
            {
                ShowMenu();
            }
            else
            {
                Pokemon p1 = Core.Player.Pokemons[_switchIndex];
                Pokemon p2 = Core.Player.Pokemons[_index];
                Core.Player.Pokemons[_switchIndex] = p2;
                Core.Player.Pokemons[_index] = p1;
                _switchIndex = -1;

                Screen.Level.OverworldPokemon.ForceTextureChange();
                Screen.Level.OverworldPokemon.Update();
                Screen.Level.OverworldPokemon.UpdateEntity();
                Screen.Level.OverworldPokemon.Render();
            }
        }
    }

    private void TakeItem()
    {
        if (Core.Player.Pokemons[_index].IsEgg == false)
        {
            if (Core.Player.Pokemons[_index].Item == null)
            {
                TextBox.Show(Core.Player.Pokemons[_index].GetDisplayName() + Localization.GetString("pokemon_screen_doesnt_hold_item"), []);
            }
            else
            {
                if (Core.Player.Pokemons[_index].Item!.AdditionalData != String.Empty)
                {
                    TextBox.Show("The Mail was taken~to your inbox on~your PC. You can view~the content there.", [], false, false);

                    Item i = Core.Player.Pokemons[_index].Item!;
                    Core.Player.Pokemons[_index].Item = null;
                    Core.Player.Mails.Add(Items.MailItem.GetMailDataFromString(i.AdditionalData));

                    _menuID = 0;
                    ChooseBox.Showing = false;
                }
                else
                {
                    Item i = Core.Player.Pokemons[_index].Item!;
                    Core.Player.Inventory.AddItem(i.ID.ToString(), 1);
                    Core.Player.Pokemons[_index].Item = null;

                    TextBox.TextColor = TextBox.PlayerColor;
                    TextBox.Show("<playername> took the~item from " + Core.Player.Pokemons[_index].GetDisplayName() + "!*" + Core.Player.Inventory.GetMessageReceive(i, 1));

                    _menuID = 0;
                    ChooseBox.Showing = false;
                }
            }
        }
        else
        {
            TextBox.Show("Eggs cannot hold items.");
        }
    }

    private void GiveItem(int itemID)
    {
        Item item = Item.GetItemByID(itemID.ToString())!;
        Pokemon pokemon = Core.Player.Pokemons[_index];

        if (pokemon.IsEgg == false)
        {
            if (item.CanBeHeld == true)
            {
                Core.Player.Inventory.RemoveItem(item.ID.ToString(), 1);

                Item? reItem = null;
                if (pokemon.Item != null)
                {
                    reItem = pokemon.Item;
                    if (reItem.AdditionalData == String.Empty)
                        Core.Player.Inventory.AddItem(reItem.ID.ToString(), 1);
                    else
                        Core.Player.Mails.Add(Items.MailItem.GetMailDataFromString(reItem.AdditionalData));
                }

                pokemon.Item = item;
                TextBox.reDelay = 0.0F;

                String t = Localization.GetString("pokemon_screen_give_item_1") + item.Name + Localization.GetString("pokemon_screen_give_item_2") + pokemon.GetDisplayName() + Localization.GetString("pokemon_screen_give_item_3");
                if (reItem != null)
                {
                    if (reItem.AdditionalData == String.Empty)
                        t += Localization.GetString("pokemon_screen_give_item_4") + reItem.Name + Localization.GetString("pokemon_screen_give_item_5");
                    else
                        t += "*The Mail was taken~to your inbox on~your PC. You can view~the content there.";
                }
                else
                {
                    t += ".";
                }

                TextBox.Show(t, []);
            }
            else
            {
                TextBox.Show(pokemon.GetDisplayName() + " cannot~hold the item~" + item.Name + ".");
            }
        }
        else
        {
            TextBox.Show("Eggs cannot hold items.");
        }

        _menuID = 0;
        ChooseBox.Showing = false;
    }

    private void CancelKeyPressed()
    {
        if (ChooseBox.Showing == true)
        {
            switch (_menuID)
            {
                case 0: ChooseBox.Showing = false; break;
                case 1: ShowMenu(); break;
            }
        }
        else
        {
            if (_switchIndex == -1)
                Core.SetScreen(PreScreen!);
            else
                _switchIndex = -1;
        }
    }

    private void ShowMenu()
    {
        _menuID = 0;
        ChooseBox.Show([Localization.GetString("pokemon_screen_summary"), Localization.GetString("pokemon_screen_switch"), Localization.GetString("pokemon_screen_item"), Localization.GetString("pokemon_screen_back")], 0, []);

        Pokemon cp = Core.Player.Pokemons[_index];
        if ((PokemonHasMove(cp, "Cut") == true && Badge.CanUseHMMove(Badge.HMMoves.Cut) == true && cp.IsEgg == false) || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            List<String> options = ChooseBox.Options.ToList(); options.Insert(1, "Cut"); ChooseBox.Options = options.ToArray();
        }
        if ((PokemonHasMove(cp, "Flash") == true && Badge.CanUseHMMove(Badge.HMMoves.Flash) == true && cp.IsEgg == false) || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            List<String> options = ChooseBox.Options.ToList(); options.Insert(1, "Flash"); ChooseBox.Options = options.ToArray();
        }
        if ((PokemonHasMove(cp, "Ride") == true && Badge.CanUseHMMove(Badge.HMMoves.Ride) == true && cp.IsEgg == false) || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            List<String> options = ChooseBox.Options.ToList(); options.Insert(1, "Ride"); ChooseBox.Options = options.ToArray();
        }
        if ((PokemonHasMove(cp, "Dig") == true && cp.IsEgg == false) || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            List<String> options = ChooseBox.Options.ToList(); options.Insert(1, "Dig"); ChooseBox.Options = options.ToArray();
        }
        if ((PokemonHasMove(cp, "Teleport") == true && cp.IsEgg == false) || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            List<String> options = ChooseBox.Options.ToList(); options.Insert(1, "Teleport"); ChooseBox.Options = options.ToArray();
        }
        if ((PokemonHasMove(cp, "Fly") == true && Badge.CanUseHMMove(Badge.HMMoves.Fly) == true && cp.IsEgg == false) || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            List<String> options = ChooseBox.Options.ToList(); options.Insert(1, "Fly"); ChooseBox.Options = options.ToArray();
        }
    }

    private void NavigateMain()
    {
        if (Controls.Right(true, false) == true) _index += 1;
        if (Controls.Left(true, false) == true) _index -= 1;
        if (Controls.Down(true, false, false) == true) _index += 2;
        if (Controls.Up(true, false, false) == true) _index -= 2;
        if (KeyBoardHandler.KeyPressed(Keys.End) == true) _index = 5;
        if (KeyBoardHandler.KeyPressed(Keys.Home) == true) _index = 0;

        if (_index < 0) _index = 0;
        else if (_index > Core.Player.Pokemons.Count - 1) _index = Core.Player.Pokemons.Count - 1;

        Player.Temp.PokemonScreenIndex = _index;
    }

    private void DrawEmptyTile(int i)
    {
        Texture2D borderTexture = TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Vector2 p = i == 0 || i == 2 || i == 4
            ? new Vector2(32, 32 + (48 + 10) * i)
            : new Vector2(416, 32 + (48 + 10) * (i - 1));
        p.X += 80; p.Y += 180;

        Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)p.X, (int)p.Y, 32, 96), new Rectangle(0, 0, 16, 48), Color.White);
        for (float x = p.X + 32; x <= p.X + 288; x += 32)
            Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)x, (int)p.Y, 32, 96), new Rectangle(16, 0, 16, 48), Color.White);
        Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)p.X + 320, (int)p.Y, 32, 96), new Rectangle(32, 0, 16, 48), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("pokemon_screen_EMPTY"), new Vector2((int)(p.X + 72), (int)(p.Y + 18)), Color.Black);
    }

    private void DrawPokemonTile(int i, Pokemon pokemon)
    {
        Texture2D borderTexture;
        if (i == _index)
        {
            borderTexture = pokemon.Status == Pokemon.StatusProblems.Fainted
                ? TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(0, 128, 48, 48), String.Empty)
                : TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(48, 0, 48, 48), String.Empty);
        }
        else
        {
            if (_switchIndex != -1 && i == _switchIndex)
            {
                borderTexture = TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            }
            else
            {
                borderTexture = pokemon.Status == Pokemon.StatusProblems.Fainted
                    ? TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(48, 48, 48, 48), String.Empty)
                    : TextureManager.GetTexture("GUI\\Menus\\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
            }
        }

        Vector2 p = i == 0 || i == 2 || i == 4
            ? new Vector2(32, 32 + (48 + 10) * i)
            : new Vector2(416, 32 + (48 + 10) * (i - 1));
        p.X += 80; p.Y += 180;

        Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)p.X, (int)p.Y, 32, 96), new Rectangle(0, 0, 16, 48), Color.White);
        for (float x = p.X + 32; x <= p.X + 288; x += 32)
            Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)x, (int)p.Y, 32, 96), new Rectangle(16, 0, 16, 48), Color.White);
        Core.SpriteBatch.Draw(borderTexture, new Rectangle((int)p.X + 320, (int)p.Y, 32, 96), new Rectangle(32, 0, 16, 48), Color.White);

        if (pokemon.IsEgg == false)
        {
            int barX = (int)((pokemon.HP / pokemon.MaxHP.Clamp(1, int.MaxValue)) * 50);
            int barPercentage = (int)((pokemon.HP / pokemon.MaxHP.Clamp(1, int.MaxValue)) * 100);
            Rectangle barRectangle = barPercentage >= 50
                ? new Rectangle(113, 0, 1, 4)
                : barPercentage > 10 ? new Rectangle(116, 0, 1, 4) : new Rectangle(115, 0, 1, 4);

            for (int x = 0; x <= barX - 1; x++)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + (x * 2) + 104), (int)(p.Y + 44), 4, 16), barRectangle, Color.White);
            for (int x = barX; x <= 49; x++)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + (x * 2) + 104), (int)(p.Y + 44), 4, 16), new Rectangle(114, 0, 1, 4), Color.White);
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 100), (int)(p.Y + 44), 4, 16), new Rectangle(112, 0, 1, 4), Color.White);
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 206), (int)(p.Y + 44), 4, 16), new Rectangle(112, 0, 1, 4), Color.White);
            Core.SpriteBatch.DrawString(FontManager.MiniFont, pokemon.HP + " / " + pokemon.MaxHP, new Vector2((int)(p.X + 120), (int)(p.Y + 64)), Color.Black);
        }

        float offset = (float)Math.Sin(_yOffset);
        if (i == _index) offset *= 3;
        if (pokemon.Status == Pokemon.StatusProblems.Fainted) offset = 0;

        Core.SpriteBatch.Draw(pokemon.GetMenuTexture(), new Rectangle((int)(p.X + 5), (int)(p.Y + offset + 10), 64, 64), BattleStats.GetStatColor(pokemon.Status));
        Core.SpriteBatch.DrawString(FontManager.MiniFont, pokemon.GetDisplayName(), new Vector2((int)(p.X + 72), (int)(p.Y + 18)), Color.Black);

        if (pokemon.IsEgg == false)
        {
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 72), (int)(p.Y + 46), 26, 12), new Rectangle(96, 10, 13, 6), Color.White);

            if (pokemon.Gender == Pokemon.Genders.Male)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + FontManager.MiniFont.MeasureString(pokemon.GetDisplayName()).X + 80), (int)(p.Y + 18), 12, 20), new Rectangle(96, 0, 6, 10), Color.White);
            else if (pokemon.Gender == Pokemon.Genders.Female)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + FontManager.MiniFont.MeasureString(pokemon.GetDisplayName()).X + 80), (int)(p.Y + 18), 12, 20), new Rectangle(102, 0, 6, 10), Color.White);
        }

        if (pokemon.Item != null && pokemon.IsEgg == false)
            Core.SpriteBatch.Draw(pokemon.Item.Texture, new Rectangle((int)(p.X + 40), (int)(p.Y + 42), 24, 24), Color.White);

        String space = String.Empty;
        for (int x = 1; x <= 3 - pokemon.Level.ToString().Length; x++)
            space += " ";

        if (pokemon.IsEgg == false)
            Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("Lv.") + space + pokemon.Level, new Vector2((int)(p.X + 14), (int)(p.Y + 64)), Color.Black);

        Texture2D? statusTexture = BattleStats.GetStatImage(pokemon.Status);
        if (statusTexture != null)
        {
            Canvas.DrawRectangle(new Rectangle((int)(p.X + 216), (int)(p.Y + 44), 42, 16), Color.Gray);
            Core.SpriteBatch.Draw(statusTexture, new Rectangle((int)(p.X + 218), (int)(p.Y + 46), 38, 12), Color.White);
        }
    }

    public override void ChangeTo()
    {
        _index = Player.Temp.PokemonScreenIndex;
    }

    private bool PokemonHasMove(Pokemon p, String moveName)
    {
        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
            return true;
        foreach (Attack a in p.Attacks)
        {
            if (a.Name.ToLower() == moveName.ToLower())
                return true;
        }
        return false;
    }

    private void UseFlash()
    {
        ChooseBox.Showing = false;
        Core.SetScreen(PreScreen!);
        if (Core.CurrentScreen.Identification == Identifications.MenuScreen)
            Core.SetScreen(Core.CurrentScreen.PreScreen!);

        if (Screen.Level.IsDark == true)
        {
            String s = "version=2" + "\n" +
                "@text.show(" + Core.Player.Pokemons[_index].GetDisplayName() + " used~Flash!)" + "\n" +
                "@environment.toggledarkness" + "\n" +
                "@sound.play(Battle\\Effects\\effect_thunderbolt)" + "\n" +
                "@text.show(The area got lit up!)" + "\n" +
                ":end";
            PlayerStatistics.Track("Flash used", 1);
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
        }
        else
        {
            String s = "version=2" + "\n" +
                "@text.show(" + Core.Player.Pokemons[_index].GetDisplayName() + " used~Flash!)" + "\n" +
                "@sound.play(Battle\\Effects\\effect_thunderbolt)" + "\n" +
                "@text.show(The area is already~lit up!)" + "\n" +
                ":end";
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
        }
    }

    private void UseFly()
    {
        if (Level.CanFly == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            ChooseBox.Showing = false;
            Core.SetScreen(PreScreen!);
            if (Core.CurrentScreen.Identification == Identifications.MenuScreen)
                Core.SetScreen(Core.CurrentScreen.PreScreen!);

            if (Screen.Level.CurrentRegion.Contains(",") == true)
            {
                List<String> regions = Screen.Level.CurrentRegion.Split(',').ToList();
                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, regions, 0, new Object[] { "Fly", Core.Player.Pokemons[_index] }), Color.White, false));
            }
            else
            {
                String startRegion = Screen.Level.CurrentRegion;
                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new MapScreen(Core.CurrentScreen, startRegion, new Object[] { "Fly", Core.Player.Pokemons[_index] }), Color.White, false));
            }
        }
        else
        {
            TextBox.Show("You cannot Fly~from here!", [], true, false);
        }
    }

    private void UseCut()
    {
        List<Entity> grassEntities = Grass.GetGrassTilesAroundPlayer(2.4F);
        if (grassEntities.Count > 0)
        {
            ChooseBox.Showing = false;
            Core.SetScreen(PreScreen!);
            if (Core.CurrentScreen.Identification == Identifications.MenuScreen)
                Core.SetScreen(Core.CurrentScreen.PreScreen!);

            PlayerStatistics.Track("Cut used", 1);
            TextBox.Show(Core.Player.Pokemons[_index].GetDisplayName() + "~used Cut!", [], true, false);
            Core.Player.Pokemons[_index].PlayCry();
            foreach (Entity e in grassEntities)
                Screen.Level.Entities.Remove(e);
        }
        else
        {
            TextBox.Show("There is nothing~to be Cut!", [], true, false);
        }
    }

    private void UseRide()
    {
        if (Screen.Level.Riding == true)
        {
            Screen.Level.Riding = false;
            Screen.Level.OwnPlayer.SetTexture(Core.Player.TempRideSkin, true);
            Core.Player.Skin = Core.Player.TempRideSkin;

            ChooseBox.Showing = false;
            Core.SetScreen(PreScreen!);
            if (Core.CurrentScreen.Identification == Identifications.MenuScreen)
                Core.SetScreen(Core.CurrentScreen.PreScreen!);

            if (Screen.Level.IsRadioOn == false || GameJolt.PokegearScreen.StationCanPlay(Screen.Level.SelectedRadioStation) == false)
                MusicManager.PlayMusic(Level.MusicLoop);
        }
        else
        {
            if (Screen.Level.Surfing == false && Screen.Camera.IsMoving == false && Screen.Camera.Turning == false && Level.CanRide() == true)
            {
                ChooseBox.Showing = false;
                Core.SetScreen(PreScreen!);
                if (Core.CurrentScreen.Identification == Identifications.MenuScreen)
                    Core.SetScreen(Core.CurrentScreen.PreScreen!);

                Screen.Level.Riding = true;
                Core.Player.TempRideSkin = Core.Player.Skin;

                String skin = "[POKEMON|";
                if (Core.Player.Pokemons[_index].IsShiny == true)
                    skin += "S]";
                else
                    skin += "N]";
                skin += Core.Player.Pokemons[_index].Number + PokemonForms.GetOverworldAddition(Core.Player.Pokemons[_index]);

                Screen.Level.OwnPlayer.SetTexture(skin, false);
                SoundManager.PlayPokemonCry(Core.Player.Pokemons[_index].Number);
                TextBox.Show(Core.Player.Pokemons[_index].GetDisplayName() + " used~Ride!", [], true, false);
                PlayerStatistics.Track("Ride used", 1);

                if (Screen.Level.IsRadioOn == false || GameJolt.PokegearScreen.StationCanPlay(Screen.Level.SelectedRadioStation) == false)
                    MusicManager.PlayMusic("ride", true);
            }
            else
            {
                TextBox.Show("You cannot Ride here!", [], true, false);
            }
        }
    }

    private void UseDig()
    {
        if (Screen.Level.CanDig == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            ChooseBox.Showing = false;
            Core.SetScreen(PreScreen!);
            if (Core.CurrentScreen.Identification == Identifications.MenuScreen)
                Core.SetScreen(Core.CurrentScreen.PreScreen!);

            bool setToFirstPerson = !((OverworldCamera)Screen.Camera).ThirdPerson;

            String s = "version=2\n" +
                "@text.show(" + Core.Player.Pokemons[_index].GetDisplayName() + " used Dig!)\n" +
                "@level.wait(20)\n" +
                "@camera.activatethirdperson\n" +
                "@camera.reset\n" +
                "@camera.fix\n" +
                "@player.turnto(0)\n" +
                "@sound.play(destroy)\n" +
                ":while:<player.position(y)>>" + (Screen.Camera.Position.Y - 1.4).ToString().ReplaceDecSeparator() + "\n" +
                "@player.turn(1)\n" +
                "@player.warp(~,~-0.1,~)\n" +
                "@level.wait(1)\n" +
                ":endwhile\n" +
                "@screen.fadeout\n" +
                "@camera.defix\n" +
                "@player.warp(" + Core.Player.LastRestPlace + "," + Core.Player.LastRestPlacePosition + ",0)\n" +
                "@player.turnto(2)";

            if (setToFirstPerson == true)
                s += "\n@camera.deactivatethirdperson";
            s += "\n@level.update\n@screen.fadein\n:end";

            PlayerStatistics.Track("Dig used", 1);
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
        }
        else
        {
            TextBox.Show("Cannot use Dig here.", [], true, false);
        }
    }

    private void UseTeleport()
    {
        if (Screen.Level.CanTeleport == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            ChooseBox.Showing = false;
            Core.SetScreen(PreScreen!);
            if (Core.CurrentScreen.Identification == Identifications.MenuScreen)
                Core.SetScreen(Core.CurrentScreen.PreScreen!);

            bool setToFirstPerson = !((OverworldCamera)Screen.Camera).ThirdPerson;
            String yFinish = (Screen.Camera.Position.Y + 2.9F).ToString().ReplaceDecSeparator();

            String s = "version=2\n" +
                "@text.show(" + Core.Player.Pokemons[_index].GetDisplayName() + "~used Teleport!)\n" +
                "@level.wait(20)\n" +
                "@camera.activatethirdperson\n" +
                "@camera.reset\n" +
                "@camera.fix\n" +
                "@player.turnto(0)\n" +
                "@sound.play(teleport)\n" +
                ":while:<player.position(y)><" + yFinish + "\n" +
                "@player.turn(1)\n" +
                "@player.warp(~,~+0.1,~)\n" +
                "@level.wait(1)\n" +
                ":endwhile\n" +
                "@screen.fadeout\n" +
                "@camera.defix\n" +
                "@player.warp(" + Core.Player.LastRestPlace + "," + Core.Player.LastRestPlacePosition + ",0)\n" +
                "@player.turnto(2)";

            if (setToFirstPerson == true)
                s += "\n@camera.deactivatethirdperson";
            s += "\n@level.update\n@screen.fadein\n:end";

            PlayerStatistics.Track("Teleport used", 1);
            ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(s, 2);
        }
        else
        {
            TextBox.Show("Cannot use Teleport here.", [], true, false);
        }
    }

    private void CheckForLegendaryEmblem()
    {
        bool hasHoOh = false, hasLugia = false, hasSuicune = false;
        foreach (Pokemon p in Core.Player.Pokemons)
        {
            switch (p.Number)
            {
                case 245: hasSuicune = true; break;
                case 249: hasLugia = true; break;
                case 250: hasHoOh = true; break;
            }
        }
        if (hasSuicune == true && hasLugia == true && hasHoOh == true)
            GameJolt.Emblem.AchieveEmblem("legendary");
    }
}
