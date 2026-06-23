using Microsoft.Xna.Framework.Media;

namespace P3D;

public class SongContainer
{
    public String Song { get; set; }
    public String Name { get; set; }
    public String Origin { get; set; }
    public TimeSpan Duration { get; set; }
    public String AudioType { get; set; }

    private Microsoft.Xna.Framework.Media.Song? _loadedSong;

    public SongContainer(String song, String name, TimeSpan duration, String origin, String audioType)
    {
        Song = song;
        Name = name;
        Origin = origin;
        Duration = duration;
        AudioType = audioType;
    }

    public Microsoft.Xna.Framework.Media.Song LoadedSong
    {
        get
        {
            if (_loadedSong == null)
                _loadedSong = Microsoft.Xna.Framework.Media.Song.FromUri(Name, new Uri(Song));
            return _loadedSong;
        }
    }

    // VB checked for a literal "intro\" (Windows path separator); use the OS separator so this also
    // matches on Linux, where Song holds a forward-slash path.
    public bool IsLoop => Song.ToLower().Contains("intro" + Path.DirectorySeparatorChar) == false && Song.ToLower().Contains("_intro") == false;

    public bool IsStandardSong => Origin == "Content";
}
