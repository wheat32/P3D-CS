using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace P3D.FontPipeline
{
    /// <summary>
    /// Model processor that forces all materials to BasicMaterialContent so the
    /// runtime BasicEffectReader is used instead of the pipeline-only ReflectiveReader.
    /// Required for OBJ models imported via Assimp, which produce MaterialContent.
    /// </summary>
    [ContentProcessor(DisplayName = "BasicEffect Model Processor")]
    public class BasicModelProcessor : ModelProcessor
    {
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            PromoteToBasicMaterial(input);
            return base.Process(input, context);
        }

        private static void PromoteToBasicMaterial(NodeContent node)
        {
            if (node is MeshContent mesh)
            {
                foreach (var geom in mesh.Geometry)
                {
                    if (geom.Material is BasicMaterialContent)
                        continue;

                    var src = geom.Material;
                    var basic = new BasicMaterialContent
                    {
                        Name = src?.Name,
                        Identity = src?.Identity,
                    };
                    if (src != null)
                    {
                        foreach (var kv in src.Textures)
                            basic.Textures[kv.Key] = kv.Value;
                    }
                    geom.Material = basic;
                }
            }
            foreach (var child in node.Children)
                PromoteToBasicMaterial(child);
        }
    }
}
