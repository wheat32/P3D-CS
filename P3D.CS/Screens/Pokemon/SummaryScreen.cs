using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D.BattleSystem;

namespace P3D;

public class SummaryScreen : Screen
{
    private Pokemon[] _party = [];
    private int _pageIndex = 0;
    private int _partyIndex = -1;
    private Pokemon? _selectedPokemon = null;

    private Texture2D _texture = null!;

    private bool _isFront = true;

    // Pointer
    private int _pointerDest = 0;
    private float _pointerPos = 0f;
    private int _pokemonDest = 0;
    private float _pokemonPos = 0f;
    private int _previewerOffset = 0;

    // Fade in animation
    private float _fadeIn = 0f;
    private float _interfaceFade = 0f;

    // Pixel animation
    private float _pixelFade = 0f;
    private Texture2D? _pixeledPokemonTexture;

    // Enroll animation
    private float _enrollY = 0f;

    private bool _closing = false;

    // Y offset for pokemon draw
    private int _yOffset = 0;

    // Page animation
    private float _pageFade = 1.0f;
    private bool _pageClosing = false;
    private bool _pageOpening = false;

    // Move display
    private int _moveIndex = 0;
    private bool _moveSelected = false;
    private float _moveFade = 0f;
    private float _moveSelectionFade = 0f;
    private float _moveSelectorPosition = 0f;

    // Move switching
    private bool _switchingMoves = false;
    private int _switchMoveIndex = -1;

    private int MenuWidth = 1104;
    private int MenuHeight = 576;
    private int DeltaX => (Core.windowSize.Width - MenuWidth) / 2;
    private int DeltaY => (Core.windowSize.Height - MenuHeight) / 2;

    public SummaryScreen(Screen currentScreen, Pokemon[] party, int partyIndex)
    {
        PreScreen = currentScreen;
        Identification = Identifications.SummaryScreen;

        _texture = TextureManager.GetTexture("GUI\\Menus\\General");

        _pageIndex = Player.Temp.PokemonSummaryPageIndex;
        _partyIndex = partyIndex;
        _party = party;

        SetDest(_partyIndex);
        GetYOffset();
        _pointerPos = _pointerDest;
        _pokemonPos = _pokemonDest;
        _moveSelectorPosition = GetMoveSelectorDest(_moveIndex);
    }

    public SummaryScreen(Screen currentScreen, Pokemon selectedPokemon)
    {
        PreScreen = currentScreen;
        Identification = Identifications.SummaryScreen;

        _texture = TextureManager.GetTexture("GUI\\Menus\\General");

        _pageIndex = Player.Temp.PokemonSummaryPageIndex;
        _selectedPokemon = selectedPokemon;

        SetDest(_partyIndex);
        GetYOffset();
        _pointerPos = _pointerDest;
        _pokemonPos = _pokemonDest;
        _moveSelectorPosition = GetMoveSelectorDest(_moveIndex);
    }

    private float GetMoveSelectorDest(int moveIndex)
    {
        return DeltaY + 76 + moveIndex * 96;
    }

    public override void Draw()
    {
        PreScreen.Draw();

        DrawGradients((int)(255 * _interfaceFade));

        DrawMain();

        if (GetPokemon().IsEgg == false)
        {
            switch (_pageIndex)
            {
                case 0:
                    DrawPage1();
                    break;
                case 1:
                    DrawPage2();
                    break;
            }
        }
        else
        {
            DrawEgg();
        }
    }

    public override void Render()
    {
        Texture2D pokemonTexture = GetPokemon().GetTexture(true);
        _pixelFade = 1; // Remove when pixel fading effect is properly implemented.

        int pixelSize = ((int)(MathHelper.Min(pokemonTexture.Width * 3, 288) * _pixelFade)).Clamp(16, MathHelper.Min(pokemonTexture.Width * 3, 288));
        if (pixelSize == MathHelper.Min(pokemonTexture.Width * 3, 288) || Core.GraphicsManager.IsFullScreen == true)
        {
            _pixeledPokemonTexture = GetPokemon().GetTexture(true);
        }
        else
        {
            RenderTarget2D pixeled = new RenderTarget2D(Core.GraphicsDevice, pixelSize, pixelSize, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Core.GraphicsDevice.SetRenderTarget(pixeled);
            Core.GraphicsDevice.Clear(Color.Transparent);
            SpriteBatch s = new SpriteBatch(Core.GraphicsDevice);
            s.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            s.Draw(pokemonTexture, new Rectangle(0, 0, pixelSize, pixelSize), Color.White);
            s.End();

            RenderTarget2D dePixeled = new RenderTarget2D(Core.GraphicsDevice, MathHelper.Min(pokemonTexture.Width * 3, 288), MathHelper.Min(pokemonTexture.Height * 3, 288), false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Core.GraphicsDevice.SetRenderTarget(dePixeled);
            Core.GraphicsDevice.Clear(Color.Transparent);
            s = new SpriteBatch(Core.GraphicsDevice);
            s.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            s.Draw(pixeled, new Rectangle(0, 0, MathHelper.Min(pokemonTexture.Width * 3, 288), MathHelper.Min(pokemonTexture.Height * 3, 288)), Color.White);
            s.End();
            _pixeledPokemonTexture = dePixeled;

            Core.GraphicsDevice.SetRenderTarget(null);
        }
    }

    private void DrawMain()
    {
        Color mainBackgroundColor = Color.White;
        if (_closing == true)
        {
            mainBackgroundColor = new Color(255, 255, 255, (int)(255 * _interfaceFade));
        }

        if (_partyIndex > -1)
        {
            for (int i = 0; i <= _party.Length - 1; i++)
            {
                double pokemonPos = GetPokemonDest(i) - 16 - (64 + 16) * (_pokemonDest - _pokemonPos);
                Texture2D pokeTexture = _party[i].GetMenuTexture();
                Vector2 pokeTextureScale = new Vector2((float)(32.0 / pokeTexture.Width * 2), (float)(32.0 / pokeTexture.Height * 2));
                Core.SpriteBatch.Draw(pokeTexture, new Rectangle((int)pokemonPos, (int)(DeltaY - 32 - pokeTexture.Height * pokeTextureScale.Y), (int)(pokeTexture.Width * pokeTextureScale.X), (int)(pokeTexture.Height * pokeTextureScale.Y)), mainBackgroundColor);
            }

            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\\Menus\\PokemonInfo"), new Rectangle((int)_pointerPos, DeltaY - 16, 32, 16), new Rectangle(0, 16, 32, 16), mainBackgroundColor);
        }

        Color onePixelLineColor = new Color(84, 198, 216);
        if (_closing == true)
        {
            onePixelLineColor.A = (byte)(255 * _interfaceFade);
        }

        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY, (int)(Math.Ceiling(MenuWidth / 16.0) * 16), 1), onePixelLineColor);

        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\PokemonInfo");

        for (int y = 0; y <= (int)_enrollY; y += 16)
        {
            for (int x = 0; x <= MenuWidth; x += 16)
            {
                Core.SpriteBatch.Draw(_texture, new Rectangle(DeltaX + x, y + DeltaY, 16, 16), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
            }
        }

        int modRes = (int)_enrollY % 16;
        if (modRes > 0)
        {
            for (int x = 0; x <= MenuWidth; x += 16)
            {
                Core.SpriteBatch.Draw(_texture, new Rectangle(DeltaX + x, (int)(_enrollY + DeltaY), 16, modRes), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
            }
        }

        if (_pageIndex == 1 && _moveSelected == true)
        {
            Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY, (int)(Math.Ceiling(MenuWidth / 16.0) * 16), (int)(Math.Ceiling(MenuHeight / 16.0) * 16) + 1), new Color(0, 0, 0, (int)(40 * _moveFade)));
            if (_partyIndex > -1)
            {
                Core.SpriteBatch.Draw(t, new Rectangle((int)_pointerPos, DeltaY - 16, 32, 16), new Rectangle(0, 16, 32, 16), new Color(0, 0, 0, (int)(40 * _moveFade)));
            }
        }

        Color shinyGradientColor = new Color(0, 0, 0, (int)(30 * _fadeIn));
        if (GetPokemon().IsShiny == true && GetPokemon().IsEgg == false)
        {
            shinyGradientColor = new Color(232, 195, 75, (int)(30 * _fadeIn));
        }

        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY, 50, MenuHeight + 16), shinyGradientColor);
        Canvas.DrawGradient(new Rectangle(DeltaX + 50, DeltaY, 272, MenuHeight + 16), shinyGradientColor, new Color(shinyGradientColor.R, shinyGradientColor.G, shinyGradientColor.B, (byte)0), true, -1);

        if (_enrollY >= 160)
        {
            Texture2D pokemonTexture = GetPokemon().GetTexture(_isFront);
            int height = ((int)(_enrollY - 160)).Clamp(0, MathHelper.Min(pokemonTexture.Height * 3, 288));

            int textureHeight = (int)(pokemonTexture.Height * (height / (float)MathHelper.Min(pokemonTexture.Height * 3, 288)));

            int pokemonTextureOffset = MathHelper.Min(pokemonTexture.Height * 3, 288) / 2;

            Core.SpriteBatch.Draw(pokemonTexture, new Rectangle(DeltaX + 152 - MathHelper.Min(pokemonTexture.Width * 3, 288) / 2 + 8, DeltaY + 218 - _yOffset - pokemonTextureOffset + 10, MathHelper.Min(pokemonTexture.Width * 3, 288), height), new Rectangle(0, 0, pokemonTexture.Width, textureHeight), new Color(0, 0, 0, 150));
            Core.SpriteBatch.Draw(pokemonTexture, new Rectangle(DeltaX + 152 - MathHelper.Min(pokemonTexture.Width * 3, 288) / 2, DeltaY + 218 - _yOffset - pokemonTextureOffset, MathHelper.Min(pokemonTexture.Width * 3, 288), height), new Rectangle(0, 0, pokemonTexture.Width, textureHeight), Color.White);
        }

        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 12, 272, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 12, 50, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);
        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 44, 272, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 44, 50, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, GetPokemon().GetDisplayName(), new Vector2(DeltaX + 8, DeltaY + 16), new Color(255, 255, 255, (int)(220 * _fadeIn)));

        if (GetPokemon().IsEgg == false)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Lv.", "Lv.") + " " + GetPokemon().Level, new Vector2(DeltaX + 50, DeltaY + 48), new Color(255, 255, 255, (int)(220 * _fadeIn)));

            Texture2D? statusTexture = BattleStats.GetStatImage(GetPokemon().Status);
            if (statusTexture != null)
            {
                Core.SpriteBatch.Draw(statusTexture, new Rectangle(DeltaX + 180, DeltaY + 48 + 3, 59, 18), new Color(255, 255, 255, (int)(255 * _fadeIn)));
            }

            if (GetPokemon().IsShiny == true)
            {
                Core.SpriteBatch.Draw(t, new Rectangle(DeltaX + 19, DeltaY + 300, 18, 18), new Rectangle(16, 0, 9, 9), new Color(255, 255, 255, (int)(255 * _fadeIn)));
            }

            switch (GetPokemon().Gender)
            {
                case Pokemon.Genders.Male:
                    Core.SpriteBatch.Draw(t, new Rectangle(DeltaX + 256 + 2, DeltaY + 16, 14, 26), new Rectangle(25, 0, 7, 13), new Color(255, 255, 255, (int)(220 * _fadeIn)));
                    break;
                case Pokemon.Genders.Female:
                    Core.SpriteBatch.Draw(t, new Rectangle(DeltaX + 256, DeltaY + 16, 18, 26), new Rectangle(32, 0, 9, 13), new Color(255, 255, 255, (int)(220 * _fadeIn)));
                    break;
            }

            if (GetPokemon().catchBall != null)
            {
                Core.SpriteBatch.Draw(GetPokemon().catchBall!.Texture, new Rectangle(DeltaX + 16, DeltaY + 48, 24, 24), new Color(255, 255, 255, (int)(255 * _fadeIn)));
            }
        }

        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 330 + 16 + 28, 272, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 330 + 16 + 28, 50, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);
        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 362 + 16 + 28, 272, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 362 + 16 + 28, 50, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);
        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 394 + 16 + 28, 272, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 394 + 16 + 28, 50, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);
        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 426 + 16 + 28, 272, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 426 + 16 + 28, 50, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);
        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 458 + 16 + 28, 272, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 458 + 16 + 28, 50, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);
        Canvas.DrawRectangle(new Rectangle(DeltaX, DeltaY + 490 + 16 + 28, 272, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)));
        Canvas.DrawGradient(new Rectangle(DeltaX + 272, DeltaY + 490 + 16 + 28, 50, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade)), new Color(0, 0, 0, 0), true, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Type", "Type"), new Vector2(DeltaX + 8, DeltaY + 330 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _interfaceFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_item", "Item"), new Vector2(DeltaX + 8, DeltaY + 362 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _interfaceFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Nature", "Nature"), new Vector2(DeltaX + 8, DeltaY + 394 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _interfaceFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_DexNo.", "Dex No."), new Vector2(DeltaX + 8, DeltaY + 426 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _interfaceFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_OT", "OT"), new Vector2(DeltaX + 8, DeltaY + 458 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _interfaceFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, GetPokemon().CatchTrainerName, new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 458 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_IDNo.", "ID No."), new Vector2(DeltaX + 8, DeltaY + 490 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _interfaceFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, GetPokemon().OT, new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 490 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));

        String pokedexNo = "???";

        if (GetPokemon().IsEgg == false)
        {
            Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle(DeltaX + 96 + 32 + 8, DeltaY + 338 + 16 + 28, 48, 16), GetPokemon().Type1.GetElementImage(), new Color(255, 255, 255, (int)(255 * _fadeIn)));
            if (GetPokemon().Type2.Type != Element.Types.Blank)
            {
                Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle(DeltaX + 152 + 32, DeltaY + 338 + 16 + 28, 48, 16), GetPokemon().Type2.GetElementImage(), new Color(255, 255, 255, (int)(255 * _fadeIn)));
            }

            if (GetPokemon().Item != null)
            {
                if (GetPokemon().Item!.IsBerry == true)
                {
                    Vector2 itemOffset = Vector2.Zero;
                    Size itemSize = new Size(GetPokemon().Item!.Texture.Width, GetPokemon().Item!.Texture.Height);
                    if (itemSize.Width != 24 || itemSize.Height != 24)
                    {
                        itemOffset.X = 24 - GetPokemon().Item!.Texture.Width;
                        itemOffset.Y = 24 - GetPokemon().Item!.Texture.Height;
                    }
                    Core.SpriteBatch.Draw(GetPokemon().Item!.Texture, new Rectangle((int)(DeltaX + 96 + 8 + 32 + itemOffset.X), (int)(DeltaY + 366 + 16 + 28 + itemOffset.Y), itemSize.Width, itemSize.Height), new Color(255, 255, 255, (int)(220 * _fadeIn)));
                    Core.SpriteBatch.DrawString(FontManager.MainFont, GetPokemon().Item!.Name.Replace("~", " "), new Vector2(DeltaX + 96 + 8 + 24 + 32, DeltaY + 366 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));
                }
                else
                {
                    Core.SpriteBatch.DrawString(FontManager.MainFont, GetPokemon().Item!.Name.Replace("~", " "), new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 366 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));
                }
            }
            else
            {
                Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("global_none", "None"), new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 366 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));
            }

            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("nature_name_" + GetPokemon().Nature.ToString(), GetPokemon().Nature.ToString()), new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 398 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));

            foreach (Pokedex pokedex in Core.Player.Pokedexes)
            {
                if (pokedex.IsActivated == true)
                {
                    String dexID = PokemonForms.GetPokemonDataFileName(GetPokemon().Number, GetPokemon().AdditionalData);
                    if (!dexID.Contains("_"))
                    {
                        String[]? additionalForms = PokemonForms.GetAdditionalDataForms(GetPokemon().Number);
                        if (additionalForms != null && additionalForms.Contains(GetPokemon().AdditionalData))
                        {
                            dexID = GetPokemon().Number + ";" + GetPokemon().AdditionalData;
                        }
                        else
                        {
                            dexID = GetPokemon().Number.ToString();
                        }
                    }
                    if (pokedex.HasPokemon(dexID, true))
                    {
                        pokedexNo = pokedex.GetPlace(dexID).ToString();
                    }
                }
            }
            while (pokedexNo.Length < 3)
            {
                pokedexNo = "0" + pokedexNo;
            }
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, "???", new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 330 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));
            Core.SpriteBatch.DrawString(FontManager.MainFont, "???", new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 362 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));
            Core.SpriteBatch.DrawString(FontManager.MainFont, "???", new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 394 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));
        }

        Core.SpriteBatch.DrawString(FontManager.MainFont, pokedexNo, new Vector2(DeltaX + 96 + 8 + 32, DeltaY + 426 + 4 + 16 + 28), new Color(255, 255, 255, (int)(220 * _fadeIn)));
    }

    private void DrawPage1()
    {
        Pokemon p = GetPokemon();

        Color[] colors = { new Color(120, 239, 155), new Color(241, 227, 154), new Color(255, 178, 114), new Color(151, 217, 205), new Color(137, 154, 255), new Color(213, 128, 255) };
        String[] statNames = { Localization.GetString("property_HP", "HP"), Localization.GetString("property_Attack", "Attack"), Localization.GetString("property_Defense", "Defense"), Localization.GetString("property_Sp_Attack", "Sp. Atk"), Localization.GetString("property_Sp_Defense", "Sp. Def"), Localization.GetString("property_Speed", "Speed") };
        String[] statValues = { p.HP + " / " + p.MaxHP, p.Attack.ToString(), p.Defense.ToString(), p.SpAttack.ToString(), p.SpDefense.ToString(), p.Speed.ToString() };
        float[] evStats = { p.EVHP, p.EVAttack, p.EVDefense, p.EVSpAttack, p.EVSpDefense, p.EVSpeed };
        float[] ivStats = { p.IVHP, p.IVAttack, p.IVDefense, p.IVSpAttack, p.IVSpDefense, p.IVSpeed };

        for (int y = 0; y <= 5; y++)
        {
            int fadeColor = 100;
            if (y % 2 == 1)
            {
                fadeColor = 70;
            }

            Color statColor = colors[y];
            statColor.A = (byte)(255 * _interfaceFade * _pageFade);

            int yOffset = 32;
            int xOffset = 72;
            int height = 32;
            if (y == 0)
            {
                xOffset = -24;
                yOffset = 0;
                height = 64;
            }

            Canvas.DrawRectangle(new Rectangle((int)(DeltaX + 344 + (380 / 2) - (264 / 2)), DeltaY + 44 + y * 32 + yOffset, 264, height), new Color(0, 0, 0, (int)(fadeColor * _interfaceFade * _pageFade)));
            Canvas.DrawRectangle(new Rectangle((int)(DeltaX + 344 + (380 / 2) - (264 / 2)), DeltaY + 44 + y * 32 + yOffset, 6, height), statColor);
            Core.SpriteBatch.DrawString(FontManager.MainFont, statNames[y], new Vector2((int)(DeltaX + 344 + (380 / 2) - (264 / 2) + 16), DeltaY + 48 + y * 32 + yOffset), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));

            float natureStatMulti = Nature.GetMultiplier(p.Nature, statNames[y]);
            Color multiColor = new Color(255, 255, 255, (int)(200 * _fadeIn * _pageFade));

            if (natureStatMulti > 1.0f)
            {
                multiColor = new Color(255, 200, 200, (int)(200 * _fadeIn * _pageFade));
            }
            else if (natureStatMulti < 1.0f)
            {
                multiColor = new Color(200, 200, 255, (int)(200 * _fadeIn * _pageFade));
            }
            Core.SpriteBatch.DrawString(FontManager.MainFont, statValues[y], new Vector2((int)(DeltaX + 344 + (380 / 2) + (264 / 2) - 128) + xOffset, DeltaY + 48 + y * 32 + yOffset), multiColor);
        }

        Texture2D pokeInfoTexture;
        if (TextureManager.TextureExist("GUI\\Menus\\PokemonInfo_" + Localization.LanguageSuffix))
        {
            pokeInfoTexture = TextureManager.GetTexture("GUI\\Menus\\PokemonInfo_" + Localization.LanguageSuffix);
        }
        else
        {
            pokeInfoTexture = TextureManager.GetTexture("GUI\\Menus\\PokemonInfo");
        }

        // HP Bar
        Core.SpriteBatch.Draw(pokeInfoTexture, new Rectangle(DeltaX + 455 + 56 - 4, DeltaY + 82, 135, 15), new Rectangle(0, 32, 90, 10), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));
        double hpV = (double)p.HP / p.MaxHP;
        int hpWidth = (int)((104 * _fadeIn) * hpV);
        int hpColorX = 0;
        if (hpV < 0.5f)
        {
            hpColorX = 5;
            if (hpV < 0.1f)
            {
                hpColorX = 10;
            }
        }
        if (p.HP > 0 && hpWidth == 0)
        {
            hpWidth = 1;
        }
        if (hpWidth > 0)
        {
            Color drawColor = Color.White;
            if (_closing == true)
            {
                drawColor = new Color(255, 255, 255, (int)(220 * _fadeIn));
            }
            drawColor.A = (byte)(drawColor.A * _pageFade);

            Core.SpriteBatch.Draw(pokeInfoTexture, new Rectangle(DeltaX + 455 + 56 + 24 - 4, DeltaY + 85, 2, 8), new Rectangle(hpColorX, 42, 2, 6), drawColor);
            Core.SpriteBatch.Draw(pokeInfoTexture, new Rectangle(DeltaX + 455 + 56 + 24 + 2 - 4, DeltaY + 85, hpWidth, 8), new Rectangle(hpColorX + 2, 42, 1, 6), drawColor);
            Core.SpriteBatch.Draw(pokeInfoTexture, new Rectangle(DeltaX + 455 + 56 + 24 + 2 - 4 + hpWidth, DeltaY + 85, 2, 8), new Rectangle(hpColorX + 3, 42, 2, 6), drawColor);
        }

        // Ability
        Canvas.DrawRectangle(new Rectangle(DeltaX + 344, DeltaY + 362 - 64, 380, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 344, DeltaY + 394 - 64, 380, 64 + 12 + 16), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Ability", "Ability"), new Vector2(DeltaX + 344 + 8, DeltaY + 366 - 64), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("ability_name_" + p.Ability.ID.ToString(), p.Ability.Name), new Vector2(DeltaX + 344 + 8 + FontManager.MainFont.MeasureString(Localization.GetString("property_Ability", "Ability")).X + 8, DeltaY + 366 - 64), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("ability_desc_" + p.Ability.ID.ToString(), p.Ability.Description).CropStringToWidth(FontManager.MainFont, 1.0f, 380 - 32), new Vector2(DeltaX + 344 + 8, DeltaY + 338), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade)));

        // Catch Method
        Canvas.DrawRectangle(new Rectangle(DeltaX + 344, DeltaY + 362 + 64 + 16, 380, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 344, DeltaY + 394 + 64 + 16, 380, 64 + 28), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_CatchMethod", "Catch Method"), new Vector2(DeltaX + 344 + 8, DeltaY + 366 + 64 + 16), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));

        if (p.CatchMethod == String.Empty)
        {
            p.CatchMethod = Localization.GetString("CatchMethod_Empty", "Somehow obtained at");
        }
        if (p.CatchLocation == String.Empty)
        {
            p.CatchLocation = Localization.GetString("CatchLocation_Empty", "an unknown place");
        }

        String catchText = p.CatchMethod.Replace(p.CatchMethod[0], Char.ToUpper(p.CatchMethod[0])) + " " + p.CatchLocation + ".";
        Core.SpriteBatch.DrawString(FontManager.MainFont, catchText.CropStringToWidth(FontManager.MainFont, 1.0f, 380 - 32), new Vector2(DeltaX + 344 + 8, DeltaY + 338 + 128 + 16), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade)));

        // EV/IV values
        Canvas.DrawRectangle(new Rectangle(DeltaX + 350 + 380 + 16, DeltaY + 362 - 64, 348, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 350 + 380 + 16, DeltaY + 394 - 64, 348, 64 + 12 + 16), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("summary_EVsIVs", "EVs (white) / IVs (blue)"), new Vector2((int)(DeltaX + 350 + 380 + 16 + 8), DeltaY + 366 - 64), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));

        for (int i = 0; i <= 5; i++)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, evStats[i].ToString(), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2 - 2.5 * 56) + i * 56 - (int)(FontManager.MainFont.MeasureString(evStats[i].ToString()).X * 0.5)), DeltaY + 338), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));
            Core.SpriteBatch.DrawString(FontManager.MainFont, ivStats[i].ToString(), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2 - 2.5 * 56) + i * 56 - (int)(FontManager.MainFont.MeasureString(ivStats[i].ToString()).X * 0.5)), DeltaY + 338 + 32 + 8), new Color(84, 198, 216, (int)(255 * _interfaceFade * _pageFade)));
        }

        // EXP
        Canvas.DrawRectangle(new Rectangle(DeltaX + 350 + 380 + 16, DeltaY + 362 + 64 + 16, 300 + 48, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_ExpPoints", "Exp. Points"), new Vector2(DeltaX + 360 + 380 + 16, DeltaY + 366 + 64 + 16), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, p.Experience.ToString(), new Vector2(DeltaX + 360 + 380 + 16 + FontManager.MainFont.MeasureString(Localization.GetString("property_ExpPoints", "Exp. Points")).X + 16, DeltaY + 366 + 64 + 16), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade)));

        if (p.Level < int.Parse(GameModeManager.GetGameRuleValue("MaxLevel", "100")))
        {
            Canvas.DrawRectangle(new Rectangle(DeltaX + 350 + 380 + 16, DeltaY + 394 + 64 + 16, 300 + 48, 64 + 28), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade)));
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("summary_ExpToNextLv", "To Next Lv."), new Vector2(DeltaX + 360 + 380 + 16, DeltaY + 398 + 64 + 16), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));

            if (p.NeedExperience(p.Level + 1) - p.Experience > 0)
            {
                Core.SpriteBatch.DrawString(FontManager.MainFont, (p.NeedExperience(p.Level + 1) - p.Experience).ToString(), new Vector2(DeltaX + 360 + 380 + 16 + FontManager.MainFont.MeasureString(Localization.GetString("summary_ExpToNextLv", "To Next Lv.")).X + 16, DeltaY + 398 + 64 + 16), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade)));

                double expV = (double)(p.Experience - p.NeedExperience(p.Level)) / (p.NeedExperience(p.Level + 1) - p.NeedExperience(p.Level));
                if (p.Level == 1)
                {
                    expV = (double)p.Experience / p.NeedExperience(2);
                }
                int expWidth = (int)((142 * _fadeIn) * expV);
                if (p.Experience > p.NeedExperience(p.Level) && expWidth == 0)
                {
                    expWidth = 1;
                }
                if (p.Experience > p.NeedExperience(p.Level + 1))
                {
                    expWidth = 142;
                    expV = 1.0f;
                }

                Color expLow = new Color(47, 204, 208, (int)(220 * _interfaceFade * _pageFade));
                Color expToColor = new Color((int)MathHelper.Lerp(47, 7, (float)expV), (int)MathHelper.Lerp(204, 48, (float)expV), (int)MathHelper.Lerp(208, 216, (float)expV), (int)(220 * _interfaceFade * _pageFade));

                Core.SpriteBatch.Draw(pokeInfoTexture, new Rectangle(DeltaX + 746 + 64, DeltaY + 411 + 24 + 64 + 16, 188, 16), new Rectangle(0, 48, 94, 8), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));
                if (expWidth > 0)
                {
                    Canvas.DrawGradient(new Rectangle(DeltaX + 746 + 64 + 42, DeltaY + 411 + 4 + 24 + 64 + 16, expWidth, 8), expLow, expToColor, true, -1);
                }
            }
        }

        // EV/IV stat graph
        Canvas.DrawRectangle(new Rectangle((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2)), DeltaY + 44, 298, 224), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade)));

        Canvas.DrawLine(new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2)), DeltaY + 44), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2)), DeltaY + 268), 2.0);
        Canvas.DrawLine(new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 300), DeltaY + 44), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 300), DeltaY + 268), 2.0);
        Canvas.DrawLine(new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) - 2), DeltaY + 268), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 300), DeltaY + 268), 2.0);

        for (int i = 0; i <= 5; i++)
        {
            Color c = colors[i];

            Canvas.DrawLine(new Color(c.R, c.G, c.B, (int)(220 * _interfaceFade * _pageFade)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 7) + i * 56, DeltaY + 268), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 7) + i * 56, DeltaY + 268 - (224 * _fadeIn)), 3.0);

            if (i < 5)
            {
                double EVcurrentPointM = evStats[i] / 256.0;
                double EVnextPointM = evStats[i + 1] / 256.0;

                Canvas.DrawLine(new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 8) + i * 56, DeltaY + 268 - (float)((224 * _fadeIn) * EVcurrentPointM)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 8) + (i + 1) * 56, DeltaY + 268 - (float)((224 * _fadeIn) * EVnextPointM)), 2.0);

                double IVcurrentPointM = ivStats[i] / 31.0;
                double IVnextPointM = ivStats[i + 1] / 31.0;

                Canvas.DrawLine(new Color(84, 198, 216, (int)(255 * _interfaceFade * _pageFade)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 8) + i * 56, DeltaY + 268 - (float)((224 * _fadeIn) * IVcurrentPointM)), new Vector2((int)(DeltaX + 350 + 380 + 16 + (348 / 2) - (298 / 2) + 8) + (i + 1) * 56, DeltaY + 268 - (float)((224 * _fadeIn) * IVnextPointM)), 2.0);
            }
        }

        // Switch Page Hint
        String viewMovesHint = ScriptVersion2.ScriptCommander.Parse(Localization.GetString("summary_hint_ViewMoves", "View Moves: [<system.button(movebackward)>] / Down")).ToString() ?? String.Empty;
        Canvas.DrawRectangle(new Rectangle((int)(DeltaX + 344 + (380 / 2) - (348 / 2)), DeltaY + 6, 300 + 48, 32), new Color(0, 0, 0, (int)(50 * _interfaceFade * _pageFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, viewMovesHint, new Vector2((int)(DeltaX + 344 + (380 / 2) - FontManager.MainFont.MeasureString(viewMovesHint).X / 2), DeltaY + 10), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));
    }

    private void DrawPage2()
    {
        Pokemon p = GetPokemon();

        for (int i = 0; i <= 3; i++)
        {
            Vector2 pos = new Vector2(DeltaX + 350, DeltaY + 76 + i * 96);
            Color c = new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade));

            Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X, (int)pos.Y, 64, 64), new Rectangle(16, 16, 16, 16), c);
            Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X + 64, (int)pos.Y, 64 * 3, 64), new Rectangle(32, 16, 16, 16), c);
            Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X + 64 * 4, (int)pos.Y, 64, 64), new Rectangle(16, 16, 16, 16), c, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);

            if (p.Attacks.Count - 1 >= i)
            {
                Core.SpriteBatch.DrawString(FontManager.MainFont, p.Attacks[i].Name, new Vector2(pos.X + 24, pos.Y + 8), new Color(0, 0, 0, (int)(220 * _fadeIn * _pageFade)));
                Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle((int)(pos.X + 26), (int)(pos.Y + 36), 48, 16), p.Attacks[i].Type.GetElementImage(), new Color(255, 255, 255, (int)(255 * _fadeIn * _pageFade)));
                Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + " " + p.Attacks[i].CurrentPP + " / " + p.Attacks[i].MaxPP, new Vector2(pos.X + 130, pos.Y + 32), new Color(0, 0, 0, (int)(220 * _fadeIn * _pageFade)));
            }
        }

        // Move selector
        Canvas.DrawBorder(3, new Rectangle(DeltaX + 350, (int)_moveSelectorPosition, 64 * 5, 64), new Color(200, 80, 80, (int)(200 * _interfaceFade * _pageFade * _moveFade)));

        // Move switch selector
        if (_switchingMoves == true)
        {
            Canvas.DrawBorder(3, new Rectangle(DeltaX + 350 - 3, (int)(GetMoveSelectorDest(_switchMoveIndex) - 3), 64 * 5 + 6, 70), new Color(80, 80, 200, (int)(200 * _interfaceFade * _pageFade * _moveFade)));
        }

        // Selected move info
        Canvas.DrawRectangle(new Rectangle(DeltaX + 700, DeltaY + 76 + 32 * 0, 350, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade * _moveFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 700, DeltaY + 76 + 32 * 1, 350, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade * _moveFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 700, DeltaY + 76 + 32 * 2, 350, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade * _moveFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 700, DeltaY + 76 + 32 * 3, 350, 32), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade * _moveFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 700, DeltaY + 76 + 32 * 4, 350, 32), new Color(0, 0, 0, (int)(100 * _interfaceFade * _pageFade * _moveFade)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 700, DeltaY + 76 + 32 * 5, 350, 240), new Color(0, 0, 0, (int)(70 * _interfaceFade * _pageFade * _moveFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Type", "Type") + ":", new Vector2(DeltaX + 710, DeltaY + 80), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade * _moveFade)));
        Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath()), new Rectangle(DeltaX + 840 + 56, DeltaY + 86, 48, 16), p.Attacks[_moveIndex].Type.GetElementImage(), new Color(255, 255, 255, (int)(255 * _fadeIn * _pageFade * _moveFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + ":", new Vector2(DeltaX + 710, DeltaY + 114), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade * _moveFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, p.Attacks[_moveIndex].CurrentPP + " / " + p.Attacks[_moveIndex].MaxPP, new Vector2(DeltaX + 840 + 56, DeltaY + 114), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade * _moveFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Category", "Category") + ":", new Vector2(DeltaX + 710, DeltaY + 144), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade * _moveFade)));
        Core.SpriteBatch.Draw(p.Attacks[_moveIndex].GetDamageCategoryImage(), new Rectangle(DeltaX + 840 + 56, DeltaY + 145, 48, 24), new Color(255, 255, 255, (int)(255 * _fadeIn * _pageFade * _moveFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Power", "Power") + ":", new Vector2(DeltaX + 710, DeltaY + 176), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade * _moveFade)));

        String power = p.Attacks[_moveIndex].Power.ToString();
        if (p.Attacks[_moveIndex].Power <= 0)
        {
            power = "-";
        }
        Core.SpriteBatch.DrawString(FontManager.MainFont, power, new Vector2(DeltaX + 840 + 56, DeltaY + 176), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade * _moveFade)));

        String accuracy = p.Attacks[_moveIndex].Accuracy.ToString();
        if (p.Attacks[_moveIndex].Accuracy <= 0)
        {
            accuracy = "-";
        }

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_Accuracy", "Accuracy") + ":", new Vector2(DeltaX + 710, DeltaY + 208), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade * _moveFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, accuracy, new Vector2(DeltaX + 840 + 56, DeltaY + 208), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade * _moveFade)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, p.Attacks[_moveIndex].Description.CropStringToWidth(FontManager.MainFont, 300), new Vector2(DeltaX + 710, DeltaY + 240), new Color(255, 255, 255, (int)(220 * _fadeIn * _pageFade * _moveFade)), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

        // Switch Page Hint
        String viewInfoHint = ScriptVersion2.ScriptCommander.Parse(Localization.GetString("summary_hint_ViewInfo", "View Info: [<system.button(moveforward)>] / Up")).ToString() ?? String.Empty;
        Canvas.DrawRectangle(new Rectangle((int)(DeltaX + 344 + (380 / 2) - (348 / 2)), DeltaY + 6, 300 + 48, 32), new Color(0, 0, 0, (int)(50 * _interfaceFade * _pageFade)));
        Core.SpriteBatch.DrawString(FontManager.MainFont, viewInfoHint, new Vector2((int)(DeltaX + 344 + (380 / 2) - FontManager.MainFont.MeasureString(viewInfoHint).X / 2), DeltaY + 10), new Color(255, 255, 255, (int)(220 * _interfaceFade * _pageFade)));
    }

    private void DrawEgg()
    {
        String s = String.Empty;
        int percent = (int)((GetPokemon().EggSteps / (float)GetPokemon().BaseEggSteps) * 100);
        if (percent <= 33)
        {
            s = Localization.GetString("summary_EggHatch_LongTime", "It looks like this Egg will take a long time to hatch.");
        }
        else if (percent > 33 && percent <= 66)
        {
            s = Localization.GetString("summary_EggHatch_MediumTime", "It's getting warmer and moves a little. It will hatch soon.");
        }
        else
        {
            s = Localization.GetString("summary_EggHatch_ShortTime", "There is strong movement noticeable. It will hatch soon!");
        }

        Canvas.DrawRectangle(new Rectangle(DeltaX + 400, DeltaY + 76, 350, 32), new Color(0, 0, 0, (int)(100 * _fadeIn)));
        Canvas.DrawRectangle(new Rectangle(DeltaX + 400, DeltaY + 108, 350, 96), new Color(0, 0, 0, (int)(70 * _fadeIn)));

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("summary_TheEggWatch", "The Egg Watch"), new Vector2(DeltaX + 410, DeltaY + 80), new Color(255, 255, 255, (int)(220 * _fadeIn)));
        String sCropped = s.CropStringToWidth(FontManager.MainFont, 1.0f, 330);
        Core.SpriteBatch.DrawString(FontManager.MainFont, sCropped, new Vector2(DeltaX + 410, DeltaY + 108 + 48 - FontManager.MainFont.MeasureString(sCropped).Y / 2), new Color(255, 255, 255, (int)(220 * _fadeIn)));
    }

    private void GetYOffset()
    {
        Texture2D t = GetPokemon().GetTexture(true);
        _yOffset = -1;

        Color[] cArr = new Color[t.Width * t.Height];
        t.GetData(cArr);

        for (int y = 0; y <= t.Height - 1; y++)
        {
            for (int x = 0; x <= t.Width - 1; x++)
            {
                if (cArr[x + y * t.Height] != Color.Transparent)
                {
                    _yOffset = y;
                    break;
                }
            }

            if (_yOffset != -1)
            {
                break;
            }
        }
    }

    public override void Update()
    {
        if (_closing == true)
        {
            if (_fadeIn > 0f)
            {
                _fadeIn = MathHelper.Lerp(0, _fadeIn, 0.8f);
                if (_fadeIn < 0f)
                {
                    _fadeIn = 0f;
                }
            }
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
            int enrollYDest = MenuHeight;
            if (_enrollY < enrollYDest)
            {
                _enrollY = MathHelper.Lerp(enrollYDest, _enrollY, 0.8f);
                if (_enrollY >= enrollYDest)
                {
                    _enrollY = enrollYDest;
                }
            }
            if (_fadeIn < 1.0f)
            {
                _fadeIn = MathHelper.Lerp(1.0f, _fadeIn, 0.95f);
                if (_fadeIn > 1.0f)
                {
                    _fadeIn = 1.0f;
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
            if (_pixelFade < 1.0f)
            {
                _pixelFade += 0.03f;
                if (_pixelFade >= 1.0f)
                {
                    _pixelFade = 1.0f;
                }
            }

            if (_pageOpening == false && _pageClosing == false && _partyIndex > -1 && _moveSelected == false)
            {
                if (_party.Length > 1)
                {
                    if (Controls.Left(true, true, false, true, true, true) || ControllerHandler.ButtonPressed(Buttons.LeftShoulder))
                    {
                        if (_partyIndex > 0)
                        {
                            _partyIndex -= 1;
                            GetYOffset();
                            SetDest(_partyIndex);
                            _isFront = true;
                        }
                    }
                    if (Controls.Right(true, true, false, true, true, true) || ControllerHandler.ButtonPressed(Buttons.RightShoulder))
                    {
                        if (_partyIndex < _party.Length - 1)
                        {
                            _partyIndex += 1;
                            GetYOffset();
                            SetDest(_partyIndex);
                            _isFront = true;
                        }
                    }
                }
            }
            if (_moveSelected == false)
            {
                if (GetPokemon().IsEgg == false)
                {
                    if (Controls.Down(true, true, false, true, true, true) == true)
                    {
                        if (_pageIndex == 0)
                        {
                            _pageClosing = true;
                            _pageOpening = false;
                        }
                    }
                    if (Controls.Up(true, true, false, true, true, true) == true)
                    {
                        if (_pageIndex == 1)
                        {
                            _pageClosing = true;
                            _pageOpening = false;
                        }
                    }
                    if (Controls.Accept() == true)
                    {
                        if (_pageIndex == 0)
                        {
                            _isFront = !_isFront;
                            SoundManager.PlaySound("select");
                        }
                        else if (_pageIndex == 1)
                        {
                            SoundManager.PlaySound("select");
                            _moveSelected = true;
                        }
                    }
                }
                if (Controls.Dismiss() == true)
                {
                    SoundManager.PlaySound("select");
                    _closing = true;
                }
            }
            else
            {
                if (Math.Abs(_moveSelectorPosition - GetMoveSelectorDest(_moveIndex)) < 32.0f)
                {
                    if (Controls.Down(true, true, true, true, true, true) == true)
                    {
                        _moveIndex += 1;

                        if (_moveIndex > GetPokemon().Attacks.Count - 1)
                        {
                            _moveIndex = 0;
                        }
                    }
                    if (Controls.Up(true, true, true, true, true, true) == true)
                    {
                        _moveIndex -= 1;

                        if (_moveIndex < 0)
                        {
                            _moveIndex = GetPokemon().Attacks.Count - 1;
                        }
                    }
                }
                if (Controls.Accept() == true && _pageIndex == 1 && GetPokemon().IsEgg == false)
                {
                    if (_switchingMoves == true)
                    {
                        Attack switchingMove = GetPokemon().Attacks[_switchMoveIndex];
                        GetPokemon().Attacks.RemoveAt(_switchMoveIndex);
                        GetPokemon().Attacks.Insert(_moveIndex, switchingMove);
                        SoundManager.PlaySound("select");
                        _switchingMoves = false;
                        _switchMoveIndex = -1;
                    }
                    else
                    {
                        if (GetPokemon().Attacks.Count > 1)
                        {
                            _switchingMoves = true;
                            _switchMoveIndex = _moveIndex;
                        }
                    }
                }
                if (Controls.Dismiss() == true)
                {
                    if (_switchingMoves == true)
                    {
                        _switchingMoves = false;
                        _switchMoveIndex = -1;
                        SoundManager.PlaySound("select");
                    }
                    else
                    {
                        _moveSelected = false;
                        SoundManager.PlaySound("select");
                    }
                }
            }
        }

        _moveIndex = _moveIndex.Clamp(0, GetPokemon().Attacks.Count - 1);
        if (_moveSelectorPosition != GetMoveSelectorDest(_moveIndex))
        {
            _moveSelectorPosition = MathHelper.Lerp(GetMoveSelectorDest(_moveIndex), _moveSelectorPosition, 0.8f);
            if (Math.Abs(_moveSelectorPosition - GetMoveSelectorDest(_moveIndex)) < 0.05f)
            {
                _moveSelectorPosition = GetMoveSelectorDest(_moveIndex);
            }
        }

        if (_pageClosing == true)
        {
            if (_pageFade >= 0f)
            {
                _pageFade -= 0.07f;
                if (_pageFade <= 0f)
                {
                    _pageFade = 0f;
                    _pageClosing = false;
                    _pageOpening = true;
                    if (_pageIndex == 0)
                    {
                        _pageIndex = 1;
                    }
                    else
                    {
                        _pageIndex = 0;
                    }
                }
            }
        }
        if (_pageOpening == true)
        {
            if (_pageFade <= 1.0f)
            {
                _pageFade += 0.07f;
                if (_pageFade >= 1.0f)
                {
                    _pageFade = 1.0f;
                    _pageClosing = false;
                    _pageOpening = false;
                }
            }
        }

        if (_pageIndex == 0)
        {
            _moveSelected = false;
            _moveIndex = 0;
            _moveFade = 0f;
            _switchingMoves = false;
            _switchMoveIndex = -1;
        }
        else
        {
            if (_moveSelected == true && _moveFade <= 1.0f)
            {
                _moveFade = MathHelper.Lerp(1.0f, _moveFade, 0.8f);
                if (_moveFade >= 0.95f)
                {
                    _moveFade = 1.0f;
                }
            }
            else if (_moveSelected == false && _moveFade > 0f)
            {
                _moveFade = MathHelper.Lerp(0.0f, _moveFade, 0.8f);
                if (_moveFade <= 0.05f)
                {
                    _moveFade = 0f;
                }
            }
        }

        if (_pointerPos < _pointerDest)
        {
            _pointerPos = MathHelper.Lerp(_pointerDest, _pointerPos, 0.8f);
            if (_pointerPos >= _pointerDest)
            {
                _pointerPos = _pointerDest;
            }
        }
        else if (_pointerPos > _pointerDest)
        {
            _pointerPos = MathHelper.Lerp(_pointerDest, _pointerPos, 0.8f);
            if (_pointerPos <= _pointerDest)
            {
                _pointerPos = _pointerDest;
            }
        }
        if (_pokemonPos < _pokemonDest)
        {
            _pokemonPos = MathHelper.Lerp(_pokemonDest, _pokemonPos, 0.8f);
            if (_pokemonPos >= _pokemonDest)
            {
                _pokemonPos = _pokemonDest;
            }
        }
        else if (_pokemonPos > _pokemonDest)
        {
            _pokemonPos = MathHelper.Lerp(_pokemonDest, _pokemonPos, 0.8f);
            if (_pokemonPos <= _pokemonDest)
            {
                _pokemonPos = _pokemonDest;
            }
        }
    }

    private int GetDest(int partyIndex)
    {
        double pointerX = Core.ScreenSize.Width / 2.0;
        if (_party.Length % 2 == 0)
        {
            int half = _party.Length / 2;
            pointerX += (16 + (64 + 16) * (partyIndex - half));
        }
        else
        {
            int half = (int)Math.Floor(_party.Length / 2.0);
            pointerX += (64 + 16) * (partyIndex - half);
        }

        return (int)(pointerX + 16);
    }

    private int GetPointerDest(int partyIndex)
    {
        double pointerX = GetDest(partyIndex);
        _previewerOffset = 0;
        while (pointerX > Core.ScreenSize.Width - (64 + 16))
        {
            pointerX -= (64 + 16);
            _previewerOffset -= 1;
        }
        while (pointerX < 64)
        {
            pointerX += (64 + 16);
            _previewerOffset += 1;
        }

        return (int)pointerX;
    }

    private int GetPokemonDest(int partyIndex)
    {
        double pointerX = GetDest(partyIndex);
        pointerX += (_previewerOffset * (64 + 16));
        return (int)pointerX;
    }

    private void SetDest(int partyIndex)
    {
        _fadeIn = 0f;
        _pixelFade = 0f;
        _pointerDest = GetPointerDest(partyIndex);
        _pokemonDest = _previewerOffset;
        if (GetPokemon().IsEgg == false)
        {
            GetPokemon().PlayCry();
        }
    }

    private Pokemon GetPokemon()
    {
        if (_partyIndex > -1)
        {
            return _party[_partyIndex];
        }
        else
        {
            return _selectedPokemon!;
        }
    }
}
