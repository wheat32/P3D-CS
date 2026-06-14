using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace P3D.FontPipeline;

[ContentImporter(".png", ".bmp",
    DisplayName = "Localized Font Texture Importer",
    DefaultProcessor = "LocalizedFontTextureProcessor")]
public class LocalizedFontTextureImporter : ContentImporter<LocalizedFontTextureContent>
{
    public override LocalizedFontTextureContent Import(string filename, ContentImporterContext context)
    {
        var inner = new TextureImporter();
        var texture = inner.Import(filename, context) as Texture2DContent
            ?? throw new InvalidContentException($"Font texture must be a 2D image: {filename}");

        var txtPath = Path.ChangeExtension(filename, ".txt");
        char[] characters = [];
        if (File.Exists(txtPath))
        {
            context.AddDependency(txtPath);
            characters = File.ReadAllText(txtPath, Encoding.UTF8).ToCharArray();
        }

        return new LocalizedFontTextureContent { Texture = texture, Characters = characters };
    }
}
