using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace P3D.FontPipeline;

// Intermediate type passed from importer to processor.
public class LocalizedFontTextureContent
{
    public Texture2DContent Texture { get; set; } = null!;
    public char[] Characters { get; set; } = [];
}
