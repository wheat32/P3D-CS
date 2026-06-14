using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace P3D.FontPipeline;

[ContentProcessor(DisplayName = "Localized Font Texture Processor")]
public class LocalizedFontTextureProcessor : ContentProcessor<LocalizedFontTextureContent, SpriteFontContent>
{
    [DefaultValue("?")]
    public string DefaultCharacter { get; set; } = "?";

    [DefaultValue(true)]
    public bool PremultiplyAlpha { get; set; } = true;

    [DefaultValue(TextureProcessorOutputFormat.Color)]
    public TextureProcessorOutputFormat TextureFormat { get; set; } = TextureProcessorOutputFormat.Color;

    public override SpriteFontContent Process(LocalizedFontTextureContent input, ContentProcessorContext context)
    {
        var innerProcessor = new FontTextureProcessor
        {
            FirstCharacter = ' ',
            PremultiplyAlpha = PremultiplyAlpha,
            TextureFormat = TextureFormat,
        };

        var result = innerProcessor.Process(input.Texture, context);

        var chars = input.Characters;
        if (chars.Length > 0)
        {
            int bitmapCount = result.CharacterMap.Count;
            int count = Math.Min(bitmapCount, chars.Length);

            for (int i = 0; i < count; i++)
                result.CharacterMap[i] = chars[i];

            // Trim CharacterMap so its length matches the mapped chars.
            // Glyphs/Cropping/Kerning may have extra trailing entries — they're
            // harmless because the runtime only indexes them via CharacterMap.
            if (bitmapCount > count)
                result.CharacterMap.RemoveRange(count, bitmapCount - count);

            context.Logger.LogMessage(
                $"Remapped {count} glyphs from .txt character list (bitmap={bitmapCount}, txt={chars.Length})");
        }

        char? dc = DefaultCharacter.Length == 1 ? DefaultCharacter[0] : (char?)null;
        if (dc.HasValue && result.CharacterMap.Contains(dc.Value))
            result.DefaultCharacter = dc;

        return result;
    }
}
