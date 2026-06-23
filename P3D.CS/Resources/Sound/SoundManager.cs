using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace P3D;

public static partial class SoundManager
{
    private const float POKEMON_CRY_VOLUME_MULTIPLIER = 1.0f;

    private static Dictionary<String, SoundContainer> _sounds = [];

    public static float Volume = 1.0f;
    public static bool Muted = false;

    public static DateTime DelayedDate = default;
    public static String DelayedSound = String.Empty;
    public static bool DelayedStopMusic = false;

    public static void Update()
    {
        if (DelayedDate != default && DelayedSound != String.Empty)
        {
            if (DateTime.Now >= DelayedDate)
            {
                PlaySound(DelayedSound, 0.0f, 0.0f, Volume, DelayedStopMusic);
                DelayedDate = default;
                DelayedSound = String.Empty;
                DelayedStopMusic = false;
            }
        }
    }

    private static bool AddSound(String name, bool forceReplace)
    {
        try
        {
            ContentManager cContent = ContentPackManager.GetContentManager("Sounds\\" + name, ".xnb,.wav");
            bool loadSound = false, removeSound = false;

            if (_sounds.ContainsKey(name.ToLower()) == false)
                loadSound = true;
            else if (forceReplace == true && _sounds[name.ToLower()].IsStandardSound == true)
            {
                removeSound = true;
                loadSound = true;
            }

            if (loadSound == true)
            {
                SoundEffect? sound = null;
                String soundRoot = Path.Combine(GameController.GamePath, cContent.RootDirectory, "Sounds");
                String nameNorm = name.Replace('\\', Path.DirectorySeparatorChar);
                String xnbPath = Path.Combine(soundRoot, nameNorm + ".xnb");
                String wavPath = Path.Combine(soundRoot, nameNorm + ".wav");
                String? foundWavPath = FindSoundFile(wavPath);

                if (File.Exists(xnbPath) == false)
                {
                    if (foundWavPath != null)
                    {
                        using Stream stream = File.Open(foundWavPath, FileMode.OpenOrCreate);
                        sound = SoundEffect.FromStream(stream);
                    }
                    else
                    {
                        Logger.Log(Logger.LogTypes.Warning, "SoundManager.cs: Sound at \"" + Path.Combine(soundRoot, nameNorm) + "\" was not found!");
                        return false;
                    }
                }
                else
                {
                    sound = cContent.Load<SoundEffect>("Sounds/" + name);
                }

                if (sound != null)
                {
                    if (removeSound == true) _sounds.Remove(name.ToLower());
                    _sounds.Add(name.ToLower(), new SoundContainer(sound, cContent.RootDirectory));
                }
            }
        }
        catch (Exception)
        {
            Logger.Log(Logger.LogTypes.Warning, "SoundManager.cs: File at \"Sounds\\" + name + "\" is not a valid sound file. They have to be a PCM wave file, mono or stereo, 8 or 16 bit and have to have a sample rate between 8k and 48k Hz.");
            return false;
        }
        return true;
    }

    public static void Clear() => _sounds.Clear();

    public static void PlaySound(String soundFile) =>
        PlaySound(soundFile, 0.0f, 0.0f, Volume, false);

    public static void PlaySound(String soundFile, bool stopMusic) =>
        PlaySound(soundFile, 0.0f, 0.0f, Volume, stopMusic);

    public static void PlaySound(String soundFile, float pitch, float pan, float volume, bool stopMusic)
    {
        if (Muted == true) return;

        String key = soundFile.ToLowerInvariant();
        SoundContainer? sound = GetSoundEffect(key);
        if (sound == null) return;

        try
        {
            Logger.Debug("SoundEffect [" + soundFile + "]");
            sound.Sound.Play(volume, pitch, pan);
            if (stopMusic == true)
                MusicManager.PauseForSound(sound.Sound);
        }
        catch (Exception)
        {
            Logger.Log(Logger.LogTypes.ErrorMessage, "Failed to play sound: no audio device available.");
        }
    }

    public static void LoadSounds(bool forceReplace)
    {
        String basePath = Path.Combine(GameController.GamePath, "Content", "Sounds");
        foreach (String soundFile in Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories))
        {
            if (soundFile.EndsWith(".wav") == true || soundFile.EndsWith(".xnb") == true)
            {
                String name = Path.GetRelativePath(basePath, soundFile.Substring(0, soundFile.Length - 4));
                AddSound(name, forceReplace);
            }
        }

        foreach (String c in Core.GameOptions.ContentPackNames)
        {
            String path = Path.Combine(GameController.GamePath, "ContentPacks", c, "Sounds");
            if (Directory.Exists(path) == true)
            {
                foreach (String soundFile in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    if (soundFile.EndsWith(".wav") == true || soundFile.EndsWith(".xnb") == true)
                    {
                        String name = Path.GetRelativePath(path, soundFile.Substring(0, soundFile.Length - 4));
                        AddSound(name, forceReplace);
                    }
                }
            }
        }
    }

    private static SoundContainer? GetSoundEffect(String name)
    {
        if (_sounds.ContainsKey(name.ToLower()) == true)
            return _sounds[name.ToLower()];
        if (TryAddGameModeSound(name.ToLower()) == true)
            return _sounds[name.ToLower()];
        Logger.Log(Logger.LogTypes.Warning, "SoundManager.cs: Cannot find sound file \"" + name + "\". Return nothing.");
        return null;
    }

    private static bool TryAddGameModeSound(String name)
    {
        String nameNorm = name.Replace('\\', Path.DirectorySeparatorChar);
        String defaultPath = Path.Combine(GameController.GamePath, "Content", "Sounds", nameNorm + ".wav");
        String[] gmParts = GameModeManager.ActiveGameMode.ContentPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        String modePath = Path.Combine([GameController.GamePath, ..gmParts, "Sounds", nameNorm + ".wav"]);
        if (FindSoundFile(modePath) != null || FindSoundFile(defaultPath) != null)
            return AddSound(name, false);
        return false;
    }

    // VB relied on Windows case-insensitive NTFS; this helper makes it work on Linux too.
    // Resolves each directory component case-insensitively so "Sounds/pc/turnon.wav" finds "Sounds/PC/TurnOn.wav".
    private static String? FindSoundFile(String path)
    {
        if (File.Exists(path)) return path;
        String? dir = Path.GetDirectoryName(path);
        String? fileName = Path.GetFileName(path);
        if (dir == null || fileName == null) return null;
        String? resolvedDir = ResolveDirectoryInsensitive(dir);
        if (resolvedDir == null) return null;
        return Directory.GetFiles(resolvedDir)
            .FirstOrDefault(f => String.Equals(Path.GetFileName(f), fileName, StringComparison.OrdinalIgnoreCase));
    }

    private static String? ResolveDirectoryInsensitive(String dir)
    {
        if (Directory.Exists(dir)) return dir;
        String? parent = Path.GetDirectoryName(dir);
        String? segment = Path.GetFileName(dir);
        if (parent == null || String.IsNullOrEmpty(segment)) return null;
        String? resolvedParent = ResolveDirectoryInsensitive(parent);
        if (resolvedParent == null) return null;
        return Directory.GetDirectories(resolvedParent)
            .FirstOrDefault(d => String.Equals(Path.GetFileName(d), segment, StringComparison.OrdinalIgnoreCase));
    }
}
