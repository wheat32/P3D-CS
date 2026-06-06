namespace P3D;

// TODO Phase 7: full MusicManager port (replace NAudio with MonoGame Song)
public static class MusicManager
{
    public static float MasterVolume = 0.5f;
    public static bool Muted;
    public static bool EnableLooping = true;

    public static void Setup() { }
    public static void Update() { }
    public static void PlayMusic(String name, bool loop = true) { }
}
