using Microsoft.Xna.Framework.Audio;

namespace P3D;

public class SoundContainer
{
    public SoundEffect Sound { get; set; }
    public String Origin { get; set; }

    public SoundContainer(SoundEffect sound, String origin)
    {
        Sound = sound;
        Origin = origin;
    }

    public bool IsStandardSound => Origin == "Content";
}
