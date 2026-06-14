using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace P3D;

public static class MusicManager
{
    private const float DEFAULT_FADE_SPEED = 0.5f;

    private static Dictionary<String, SongContainer> _songs = [];

    public static float Volume { get; set; } = 1.0f;
    private static float _lastVolume = 1.0f;
    private static bool _muted = false;
    private static bool _paused = false;

    public static String ForceMusic = String.Empty;

    public static bool IsLooping => Playlist != null && Playlist.Count <= 1;

    public static List<SongContainer?> Playlist { get; private set; } = [];
    public static String _currentSongName = "Silence";
    public static SongContainer? _currentSong = null;

    public static SongContainer? CurrentSong
    {
        get
        {
            if (Playlist.Count > 0 && Playlist[0] != null)
                return Playlist[0];
            return GetSong("silence");
        }
    }

    private static DateTime _pausedUntil;
    private static bool _isPausedForSound = false;

    private static DateTime _introMuteTime;
    public static DateTime _introEndTime;
    public static bool _isIntroStarted = false;
    public static String _introContinueSong = String.Empty;

    private static float _fadeSpeed = DEFAULT_FADE_SPEED;
    public static bool _isFadingIn = false;
    private static bool _isFadingOut = false;
    public static bool _isCurrentlyFading = false;

    public static bool EnableLooping = true;
    public static float PauseVolume { get; set; } = 1.0f;
    public static float MasterVolume { get; set; } = 1.0f;

    public static bool Muted
    {
        get => _muted;
        set
        {
            if (_muted != value)
            {
                _muted = value;
                if (_muted == true)
                {
                    Volume = 0.0f;
                    Core.GameMessage.ShowMessage(Localization.GetString("game_message_audio_off"), 12, FontManager.MainFont, Microsoft.Xna.Framework.Color.White);
                }
                else
                {
                    if (_isPausedForSound == true)
                    {
                        _muted = true;
                        Volume = 0.0f;
                    }
                    else
                    {
                        Volume = 1.0f;
                        Core.GameMessage.ShowMessage(Localization.GetString("game_message_audio_on"), 12, FontManager.MainFont, Microsoft.Xna.Framework.Color.White);
                    }
                }
            }
        }
    }

    public static bool Paused
    {
        get => _paused;
        set
        {
            if (_paused != value)
            {
                _paused = value;
                if (_paused == true)
                {
                    MediaPlayer.Pause();
                    _introMuteTime = DateTime.Now;
                }
                else
                {
                    ResumePlayback();
                }
            }
        }
    }

    public static void Setup()
    {
        MasterVolume = 1.0f;
        Volume = _muted ? 0.0f : 1.0f;
        Playlist = [];
        _fadeSpeed = DEFAULT_FADE_SPEED;
        _isFadingOut = false;
    }

    public static void Clear() => _songs.Clear();

    public static void ClearCurrentlyPlaying()
    {
        Playlist.Clear();
        _currentSong = null;
        _currentSongName = "Silence";
        _isIntroStarted = false;
        Volume = _muted ? 0.0f : 1.0f;
        _isFadingOut = false;
        Play("Silence", true, 0.0f);
    }

    public static void PlayNoMusic() => Play("Silence", true, 0.01f);

    public static void PlayMusic(String name, bool loop = true) => Play(name, loop);

    public static void Update()
    {
        if (_isPausedForSound == true)
        {
            if (DateTime.Now >= _pausedUntil)
            {
                if (Paused == true)
                {
                    _isPausedForSound = false;
                    Paused = false;
                }
            }
        }
        else
        {
            if (_isFadingOut == true)
            {
                _isCurrentlyFading = true;
                Volume -= _fadeSpeed;

                if (Volume <= 0.0f)
                {
                    Volume = 0.0f;
                    _isFadingOut = false;

                    SongContainer? song = Playlist.Count > 0 ? Playlist[0] : null;

                    if (song != null)
                    {
                        PlaySong(song);
                        if (_isFadingIn == true)
                        {
                            _isFadingIn = false;
                            _introEndTime = DateTime.Now + song.Duration;
                            _isIntroStarted = true;
                        }
                        Volume = _muted ? 0.0f : 1.0f;
                        _isCurrentlyFading = false;
                    }
                    else
                    {
                        _isFadingIn = false;
                        _isCurrentlyFading = false;
                        ClearCurrentlyPlaying();
                        Volume = _muted ? 0.0f : 1.0f;
                    }
                }
            }
            else
            {
                if (_isFadingIn == false)
                    _isCurrentlyFading = false;
            }

            // advance playlist when current song ends
            if (_isCurrentlyFading == false && MediaPlayer.State == MediaState.Stopped && Playlist.Count > 1)
            {
                Playlist.RemoveAt(0);
                SongContainer? next = Playlist[0];
                if (next != null)
                {
                    Logger.Debug("Play song [" + next.Name + "]");
                    _currentSongName = next.Name;
                    _currentSong = next;
                    MediaPlayer.IsRepeating = next.IsLoop && EnableLooping;
                    try { MediaPlayer.Play(next.LoadedSong); } catch (Exception) { }
                }
            }
        }

        if (GameController.IsActiveWindow() == true && _lastVolume != (Volume * PauseVolume * MasterVolume))
            UpdateVolume();
    }

    public static void UpdateVolume()
    {
        _lastVolume = Volume * PauseVolume * MasterVolume;
        MediaPlayer.Volume = Math.Clamp(_lastVolume, 0.0f, 1.0f);
    }

    public static void PauseForSound(SoundEffect sound)
    {
        _isPausedForSound = true;
        _pausedUntil = DateTime.Now + sound.Duration;
        Pause();
    }

    public static void Pause() => Paused = true;

    public static void Stop() => ClearCurrentlyPlaying();

    public static void ResumePlayback()
    {
        if (CurrentSong != null)
        {
            if (_isIntroStarted == true)
            {
                TimeSpan pauseTime = DateTime.Now.Subtract(_introMuteTime);
                _introEndTime = _introEndTime + pauseTime;
            }
            MediaPlayer.Resume();
        }
    }

    public static void SetMuted(bool value) => Muted = value;
    public static void SetPaused(bool value) => Paused = value;

    private static void PlaySong(SongContainer? song)
    {
        if (song == null)
        {
            _currentSongName = "Silence";
            _currentSong = null;
            return;
        }

        Logger.Debug("Play song [" + song.Song + "]");
        MediaPlayer.IsRepeating = song.IsLoop && EnableLooping;
        try
        {
            MediaPlayer.Play(song.LoadedSong);
        }
        catch (Exception)
        {
            Logger.Log(Logger.LogTypes.ErrorMessage, "No usable audio device");
        }
        MediaPlayer.Volume = Math.Clamp(Volume * MasterVolume, 0.0f, 1.0f);
        _currentSongName = song.Name;
        _currentSong = song;
    }

    private static void FadeInto(SongContainer? song, float fadeSpeed)
    {
        _isFadingOut = true;
        Playlist.Add(song ?? GetSong("silence"));
        _fadeSpeed = fadeSpeed;
    }

    public static SongContainer? Play(String song) => Play(song, true, DEFAULT_FADE_SPEED);

    public static SongContainer? Play(String song, bool playIntro, bool loopSong = true) =>
        Play(song, playIntro, DEFAULT_FADE_SPEED, loopSong);

    public static SongContainer? Play(String song, bool playIntro, float fadeSpeed, bool loopSong = true, String afterBattleIntroSong = "")
    {
        SongContainer? playedSong = null;

        String currentSong = GetCurrentSong().ToLowerInvariant();
        String songName = GetSongName(song);
        String afterBattleIntroSongName = GetSongName(afterBattleIntroSong);

        if (currentSong == "silence" || currentSong != songName)
        {
            if (afterBattleIntroSongName != String.Empty)
            {
                SongContainer? battleIntroSong = GetSong(songName);
                SongContainer? regularIntroSong = GetSong("intro\\" + afterBattleIntroSongName);
                SongContainer? regularLoopSong = GetSong(afterBattleIntroSongName);

                if (battleIntroSong != null && regularLoopSong != null && battleIntroSong.Origin == regularLoopSong.Origin)
                {
                    Playlist.Clear();
                    Playlist.Add(battleIntroSong);
                    if (regularIntroSong != null && regularIntroSong.Origin == regularLoopSong.Origin)
                        Playlist.Add(regularIntroSong);
                    if (regularLoopSong != null) Playlist.Add(regularLoopSong);
                    PlaySong(battleIntroSong);
                    playedSong = battleIntroSong;
                }
                else
                {
                    if (regularIntroSong != null && regularLoopSong != null && regularIntroSong.Origin == regularLoopSong.Origin)
                    {
                        Playlist.Clear();
                        Playlist.Add(regularIntroSong);
                        if (regularLoopSong != null) Playlist.Add(regularLoopSong);
                        PlaySong(regularIntroSong);
                        playedSong = regularIntroSong;
                    }
                    else if (regularLoopSong != null)
                    {
                        Playlist.Clear();
                        Playlist.Add(regularLoopSong);
                        PlaySong(regularLoopSong);
                        playedSong = regularLoopSong;
                    }
                }
            }
            else if (playIntro == true)
            {
                SongContainer? introSong = SongExists("intro\\" + songName) ? GetSong("intro\\" + songName) : null;
                SongContainer? nextSong = GetSong(songName);

                if (introSong != null && nextSong != null && introSong.Origin == nextSong.Origin)
                {
                    Playlist.Clear();
                    if (fadeSpeed > 0.0f)
                    {
                        _isIntroStarted = false;
                        _isFadingIn = true;
                        FadeInto(introSong, fadeSpeed);
                        Playlist.Add(nextSong);
                    }
                    else
                    {
                        _isIntroStarted = true;
                        _introEndTime = DateTime.Now + introSong.Duration;
                        PlaySong(introSong);
                        Playlist.AddRange([introSong, nextSong]);
                    }
                    playedSong = introSong;
                }
                else
                {
                    _isIntroStarted = false;
                    _isFadingIn = false;
                }
            }
            else
            {
                _isIntroStarted = false;
                _isFadingIn = false;
            }

            EnableLooping = loopSong;

            if (_isIntroStarted == false && _isFadingIn == false && afterBattleIntroSongName == String.Empty)
            {
                Playlist.Clear();
                SongContainer? nextSong = GetSong(song);
                if (fadeSpeed > 0.0f)
                    FadeInto(nextSong, fadeSpeed);
                else
                {
                    PlaySong(nextSong);
                    Playlist.Add(nextSong);
                }
                playedSong = nextSong;
            }
        }

        return playedSong;
    }

    public static bool SongExists(String songName, bool logIfNotFound = true) =>
        GetSong(songName, logIfNotFound) != null;

    private static String GetCurrentSong()
    {
        if (Playlist != null)
        {
            if (Playlist.Count > 1)
            {
                if (_isFadingOut == true)
                {
                    if (_isFadingIn == true)
                        return Playlist[0]?.Name ?? "Silence";
                    return Playlist[1]?.Name ?? Playlist[0]?.Name ?? "Silence";
                }
                else
                {
                    if (_isIntroStarted == true)
                        return Playlist[1]?.Name ?? Playlist[0]?.Name ?? "Silence";
                    return Playlist[0]?.Name ?? "Silence";
                }
            }
            else if (Playlist.Count == 1)
                return Playlist[0]?.Name ?? "Silence";
        }
        return "Silence";
    }

    public static SongContainer? GetSong(String songName, bool logIfNotFound = true)
    {
        String key = GetSongName(songName);
        ContentManager cContent = ContentPackManager.GetContentManager("Songs\\" + key, ".ogg,.mp3,.wma");
        String keyNorm = key.Replace('\\', Path.DirectorySeparatorChar);
        String songRoot = Path.Combine(GameController.GamePath, cContent.RootDirectory, "Songs");
        String contentPath = Path.Combine(songRoot, keyNorm);
        String[] gmParts = GameModeManager.ActiveGameMode.ContentPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        String gamemodePath = Path.Combine([GameController.GamePath, ..gmParts, "Songs", keyNorm]);
        String defaultPath = Path.Combine(GameController.GamePath, "Content", "Songs", keyNorm);

        if (_songs.ContainsKey(key) == true)
            return _songs[key];

        String audioType = String.Empty;
        foreach (String ext in new[] { ".ogg", ".mp3", ".wma" })
        {
            if (File.Exists(contentPath + ext) || File.Exists(gamemodePath + ext) || File.Exists(defaultPath + ext))
            {
                audioType = ext;
                break;
            }
        }

        if (audioType != String.Empty)
        {
            if (File.Exists(contentPath + audioType) || File.Exists(gamemodePath + audioType) || File.Exists(defaultPath + audioType))
            {
                if (AddSong(key, false) == true)
                    return _songs[key];
            }
        }
        else
        {
            if (GameController.IS_DEBUG_ACTIVE == true)
                Logger.Debug("MusicManager.vb: Cannot find music file \"" + songName + "\". Return nothing.");
            else if (songName.Contains("intro\\") == false && logIfNotFound == true)
                Logger.Log(Logger.LogTypes.Warning, "MusicManager.vb: Cannot find music file \"" + songName + "\". Return nothing.");
        }

        return null;
    }

    private static String GetSongName(String song)
    {
        String key = song.ToLowerInvariant();
        if (SongAliasMap.TryGetValue(key, out String? alias) == true)
            key = alias.ToLowerInvariant();
        return key;
    }

    private static bool AddSong(String name, bool forceReplace)
    {
        try
        {
            ContentManager cContent = ContentPackManager.GetContentManager("Songs\\" + name, ".ogg,.mp3,.wma");
            bool loadSong = false, removeSong = false;

            if (_songs.ContainsKey(GetSongName(name)) == false)
                loadSong = true;
            else if (forceReplace == true)
            {
                removeSong = true;
                loadSong = true;
            }

            if (loadSong == true)
            {
                String? songFilePath = null;
                String? audioType = null;
                String songRoot = Path.Combine(GameController.GamePath, cContent.RootDirectory, "Songs");
                String nameNorm = name.Replace('\\', Path.DirectorySeparatorChar);

                foreach (String ext in new[] { ".ogg", ".mp3", ".wma" })
                {
                    if (File.Exists(Path.Combine(songRoot, nameNorm + ext)) == true)
                    {
                        audioType = ext;
                        break;
                    }
                }

                if (audioType == null)
                {
                    Logger.Log(Logger.LogTypes.Warning, "MusicManager.vb: Song at \"" + Path.Combine(songRoot, nameNorm) + "\" was not found!");
                    return false;
                }

                songFilePath = Path.Combine(songRoot, nameNorm + audioType);
                if (removeSong == true) _songs.Remove(GetSongName(name));

                TimeSpan duration = GetSongDuration(songFilePath);
                _songs.Add(GetSongName(name), new SongContainer(songFilePath, name, duration, cContent.RootDirectory, audioType));
            }
        }
        catch (Exception)
        {
            Logger.Log(Logger.LogTypes.Warning, "MusicManager.vb: File at \"Songs\\" + name + "\" is not a valid song file!");
            return false;
        }
        return true;
    }

    public static void LoadMusic(bool forceReplace)
    {
        String[] lmParts = GameModeManager.ActiveGameMode.ContentPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        String songDir = Path.Combine([GameController.GamePath, ..lmParts, "Songs"]);
        if (Directory.Exists(songDir) == true)
        {
            foreach (String musicFile in Directory.GetFiles(songDir, "*.*", SearchOption.AllDirectories))
            {
                if (musicFile.EndsWith(".ogg") || musicFile.EndsWith(".mp3") || musicFile.EndsWith(".wma"))
                {
                    String introSep = Path.Combine("Songs", "intro") + Path.DirectorySeparatorChar;
                    String key = musicFile.Contains(introSep) == true
                        ? "intro\\" + Path.GetFileNameWithoutExtension(musicFile)
                        : Path.GetFileNameWithoutExtension(musicFile);
                    AddSong(key, forceReplace);
                }
            }
        }

        foreach (String c in Core.GameOptions.ContentPackNames)
        {
            String packPath = Path.Combine(GameController.GamePath, "ContentPacks", c, "Songs");
            if (Directory.Exists(packPath) == true)
            {
                foreach (String musicFile in Directory.GetFiles(packPath, "*.*", SearchOption.AllDirectories))
                {
                    if (musicFile.EndsWith(".ogg") || musicFile.EndsWith(".mp3") || musicFile.EndsWith(".wma"))
                    {
                        String key = musicFile.Contains("\\Songs\\intro\\") == true
                            ? "intro\\" + Path.GetFileNameWithoutExtension(musicFile)
                            : Path.GetFileNameWithoutExtension(musicFile);
                        AddSong(key, forceReplace);
                    }
                }
            }
        }
    }

    private static TimeSpan GetSongDuration(String songFilePath)
    {
        // MonoGame Song.FromUri doesn't expose duration; use a brief peek via loading
        // Duration is only needed for intro timing — return a safe default if unknown
        try
        {
            Song mgSong = Song.FromUri(Path.GetFileNameWithoutExtension(songFilePath), new Uri(songFilePath));
            return mgSong.Duration;
        }
        catch (Exception)
        {
            return TimeSpan.FromMinutes(3);
        }
    }

    private static Dictionary<String, String> SongAliasMap => new Dictionary<String, String>
    {
        { "welcome",               "RouteMusic1" },
        { "battle",                "johto_wild" },
        { "batleintro",            "johto_wild_intro" },
        { "johto_battle_intro",    "johto_wild_intro" },
        { "darkcave",              "dark_cave" },
        { "showmearound",          "show_me_around" },
        { "sprouttower",           "sprout_tower" },
        { "johto_rival_appear",    "johto_rival_encounter" },
        { "ilex_forest",           "IlexForest" },
        { "union_cave",            "IlexForest" },
        { "mt_mortar",             "IlexForest" },
        { "whirlpool_islands",     "IlexForest" },
        { "tohjo_falls",           "IlexForest" },
        { "no_music",              "Silence" },
    };
}
