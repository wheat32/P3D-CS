namespace P3D;

// TODO Phase 7: full SoundManager port (replace NAudio with MonoGame SoundEffect)
public static partial class SoundManager
{
    public static float Volume = 1.0f;
    public static bool Muted;

    public static void Update() { }
    public static void PlaySound(String name) { }
    public static void PlaySound(String name, float x, float y, float volume, bool stopMusic) { }
}
