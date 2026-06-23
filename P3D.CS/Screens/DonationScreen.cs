using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class DonationScreen : Screen
{
    private Texture2D _mainTexture = null!;
    private Texture2D _scrollTexture = null!;

    private String[] _donatorList = ["Many Anonymous Donors", "Username99", "Merder222", "Felipe 2", "Kuro95", "WheresMyTea", "RandomBounty", "NumseFisK", "abcoanon", "SirMarty", "The_Merciless95", "adm0n", "Avaluque", "Duck Tard", "L3_Purr", "Derata", "TheFlipside", "Zippo", "Dirty Harry", "Chaos7777", "Sontee", "PsYcO363", "Sammyinside", "mickeystand1", "Tripsaur", "Fox405", "LoganKnez", "Jehowi", "Sedat", "Mischapus", "PeanutButter", "Nathan Wilson", "Fluffy", "Shou Liengod", "Gorogok", "Yoshina", "Hodsy Beats", "takenbycats", "sorixkhaos", "lordkango", "northway", "bloodeyezack", "gladdy16", "Paradetheday", "Gawerty", "Haydos709", "ShadyGame", "Mikolaj Nowicki", "Koolboyman", "TrainerStan", "carebear", "Bedders", "Matz", "ITAxDarko", "Rhyinn", "arthegon", "bmalfer", "Noah Cloud", "Matti", "Yrael", "Tornado9797", "Wilkojc", "Namu", "SACooper95", "nilllzz", "Nesasio", "beenlord", "Maria", "JohnnyRooks", "Calcifer", "Nyves", "Daniel Saavedra", "DannyM93", "The-amazing-blackstar", "DevoidLight", "OhSnapItsDavid", "Anvil555", "Clanor", "Liamash3", "Daysofthenew690", "Luan Nicholas", "Pushacher19", "Meowth", "DarknessYami", "Gameshark93", "Enethil", "Gnifle", "abovo", "p1neapple", "Destructosaur", "Darkfire", "Tim Dargan", "PrincessKooh", "Tyler Snyder", "hannes3120", "Raa", "Richard Tisher", "Brutalicious", "DarkLink", "Mpilemann", "PerrBearr", "robod", "Davey", "Colin_Mg", "Whitney", "mreh", "zXxLIPSxXz", "Xane", "LeeMan", "ekwilson79", "Darrin Danhauer", "AlessaGarnish", "Sola", "Luffy343", "Masasume", "Grabsak Turnkoff", "Sporkedmango", "Splint", "Mitchmack", "Pegasuraptor", "CrayonDoctor", "Olliewott", "Maizox", "Gamester565", "Michael", "Syrca", "PaperDanie2", "Gamerunner15", "Ashurnasirpal", "edward", "Gusty Glalie", "DracoHouston", "BakaOnibi", "Tj8805", "Lunick", "Karasu416", "Steven Sinclair", "Corbin Lair", "Michael Langen", "Diego López", "Sam Schultz", "Tom Bolen", "Lewis Thompson", "William Hafey", "Edward Akus", "Arno Wendorff", "Kim Nay", "Danie Daniels", "Joe Palacios", "Stuart Oxtoby", "Jack Mckenzie", "Michael Cutipa", "The Homies", "Alicia Barfoot", "Maintrain97", "Shinytish", "Michael Molina", "Edward Akus"];

    private int _offsetY = 0;

    public DonationScreen(Screen currentScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.DonationScreen;
        _mainTexture = TextureManager.GetTexture("House", new Rectangle(83, 98, 10, 12));
        _scrollTexture = TextureManager.GetTexture("GUI\\Menus\\Menu");
    }

    public override void Update()
    {
        if (Controls.Up(true, true, true, true) == true) _offsetY -= 1;
        if (Controls.Down(true, true, true, true) == true) _offsetY += 1;
        _offsetY = (int)MathHelper.Clamp(_offsetY, 0, _donatorList.Length - 12);

        if (Controls.Dismiss() == true)
            Core.SetScreen(PreScreen!);
    }

    public override void Draw()
    {
        PreScreen!.Draw();
        Core.SpriteBatch.Draw(_mainTexture, new Rectangle(Core.windowSize.Width / 2 - 285, 0, 570, 680), Color.White);

        String t = String.Empty;
        for (int i = _offsetY; i <= 11 + _offsetY; i++)
        {
            if (i != _offsetY) t += Environment.NewLine + Environment.NewLine;
            if (_donatorList.Length - 1 >= i) t += _donatorList[i];
        }

        if (_donatorList.Length > 12)
        {
            Canvas.DrawScrollBar(new Vector2(Core.windowSize.Width / 2 + 180, 100), _donatorList.Length, 12, _offsetY, new Size(4, 500), false,
                TextureManager.GetTexture(_scrollTexture, new Rectangle(112, 12, 1, 1)),
                TextureManager.GetTexture(_scrollTexture, new Rectangle(113, 12, 1, 1)));
        }

        Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(Core.windowSize.Width / 2 - 180, 100), Color.Black);
        Canvas.DrawRectangle(new Rectangle(Core.windowSize.Width / 2 - 285, 0, 570, 57), new Color(56, 56, 56));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("donation_screen_donators") + ": ", new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.MainFont.MeasureString("Donators:").X / 2), 20), Color.White);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("donation_screen_backadvice"), new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.MainFont.MeasureString("Press E to close").X / 2), 640), Color.White);
    }
}
