namespace P3D;

// TODO Phase 7: full MusicManager port (replace NAudio with MonoGame Song)
public static class MusicManager
{
    public static float MasterVolume = 0.5f;
    public static bool Muted;
    public static bool Paused;
    public static bool EnableLooping = true;
    public static String ForceMusic = String.Empty;
    public static String _currentSongName = String.Empty;
    public static SongContainer? CurrentSong { get; private set; }

    public static void Setup() { }
    public static void Update() { }
    public static void PlayMusic(String name, bool loop = true) { }
    public static void Play(String name, bool loop = true, float fadeIn = 0f) { _currentSongName = name; }
    public static void Play(String name, bool forceUpdate, bool loop) { _currentSongName = name; }
    public static void Play(String name, bool forceUpdate, float fadeIn, bool loop) { _currentSongName = name; }
    public static void SetMuted(bool value) { Muted = value; }
    public static void SetPaused(bool value) { Paused = value; }
    public static SongContainer? GetSong(String name) => null;
    public static void Stop() { }
}

// TODO Phase 7: full SongContainer port
public class SongContainer
{
    public String Name { get; set; } = String.Empty;
}
