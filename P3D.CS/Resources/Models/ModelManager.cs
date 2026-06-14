using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class ModelManager
{
    private static Dictionary<String, Model> _modelList = [];
    public const float MODELSCALE = 0.00625f;

    public static Model? GetModel(String path)
    {
        ContentManager cContent = ContentPackManager.GetContentManager(path, ".xnb");
        String tKey = cContent.RootDirectory + "\\" + path;

        if (_modelList.ContainsKey(tKey) == false)
        {
            Model m = cContent.Load<Model>(path);
            _modelList.Add(tKey, CreateShallowCopy(m));
        }

        return _modelList[tKey];
    }

    private static Model CreateShallowCopy(Model m)
    {
        MethodInfo? method = m.GetType().GetMethod("MemberwiseClone",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        return (Model)method!.Invoke(m, [])!;
    }

    public static bool ModelExist(String path)
    {
        ContentManager cContent = ContentPackManager.GetContentManager(path, ".xnb");
        if (cContent.RootDirectory == "Content" && GameModeManager.ActiveGameMode.DirectoryName != "Kolben")
            return false;
        if (path == String.Empty) return false;
        return File.Exists(Path.Combine(GameController.GamePath, cContent.RootDirectory, path.Replace('\\', Path.DirectorySeparatorChar) + ".xnb"));
    }

    public static float PokeModelScale(String path)
    {
        ContentManager cContent = ContentPackManager.GetContentManager(path, ".xnb");
        if (cContent.RootDirectory == "Content" && GameModeManager.ActiveGameMode.DirectoryName != "Kolben")
            return 1.0f;
        if (path == String.Empty) return 1.0f;

        if (File.Exists(Path.Combine(GameController.GamePath, cContent.RootDirectory, path.Replace('\\', Path.DirectorySeparatorChar) + ".xnb")) == true)
        {
            if (cContent.RootDirectory.ToLower().Contains("contentpacks\\") == true)
            {
                if (ContentPackManager.PokeModelScale.ContainsKey(cContent.RootDirectory.ToLower()) == true)
                    return ContentPackManager.PokeModelScale[cContent.RootDirectory.ToLower()];
                return 1.0f;
            }
            else
            {
                String rootDirectory = GameModeManager.ActiveGameMode.ContentPath.Remove(0, 1);
                if (rootDirectory.EndsWith("\\") == true)
                    rootDirectory = rootDirectory.Remove(rootDirectory.Length - 1, 1);
                if (cContent.RootDirectory.ToLower().Contains(rootDirectory.ToLower()) == true)
                    return GameModeManager.ActiveGameMode.PokeModelScale;
                return 1.0f;
            }
        }
        return 1.0f;
    }

    public static Vector3 PokeModelRotation(String path)
    {
        ContentManager cContent = ContentPackManager.GetContentManager(path, ".xnb");
        if (cContent.RootDirectory == "Content" && GameModeManager.ActiveGameMode.DirectoryName != "Kolben")
            return Vector3.Zero;
        if (path == String.Empty) return Vector3.Zero;

        if (File.Exists(Path.Combine(GameController.GamePath, cContent.RootDirectory, path.Replace('\\', Path.DirectorySeparatorChar) + ".xnb")) == true)
        {
            if (cContent.RootDirectory.ToLower().Contains("contentpacks\\") == true)
            {
                if (ContentPackManager.PokeModelRotation.ContainsKey(cContent.RootDirectory.ToLower()) == true)
                    return ContentPackManager.PokeModelRotation[cContent.RootDirectory.ToLower()];
                return Vector3.Zero;
            }
            else
            {
                String rootDirectory = GameModeManager.ActiveGameMode.ContentPath.Remove(0, 1);
                if (rootDirectory.EndsWith("\\") == true)
                    rootDirectory = rootDirectory.Remove(rootDirectory.Length - 1, 1);
                if (cContent.RootDirectory.ToLower().Contains(rootDirectory.ToLower()) == true)
                    return GameModeManager.ActiveGameMode.PokeModelRotation;
                return Vector3.Zero;
            }
        }
        return Vector3.Zero;
    }

    public static void Clear() => _modelList.Clear();

    public static Texture2D DrawModelToTexture(String modelName, RenderTarget2D renderTarget,
        Vector3 modelPosition, Vector3 cameraPosition, Vector3 cameraRotation, float scale, bool enableLight)
    {
        float _scale = scale * MODELSCALE;
        Core.GraphicsDevice.SetRenderTarget(renderTarget);
        Core.GraphicsDevice.Clear(Color.Transparent);
        Core.GraphicsDevice.BlendState = BlendState.Opaque;
        Core.GraphicsDevice.SamplerStates[0] = Core.Sampler;

        Model? m = GetModel(modelName);
        if (m == null) return renderTarget;

        if (enableLight == true)
        {
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    if (part.Effect.GetType() == typeof(BasicEffect))
                        part.Effect = new BasicEffectWithAlphaTest((BasicEffect)part.Effect);
                }
                foreach (BasicEffectWithAlphaTest e in mesh.Effects.OfType<BasicEffectWithAlphaTest>())
                {
                    BasicEffectWithAlphaTest effect = e;
                    Lighting.UpdateLighting(ref effect, true);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.7f);
                    effect.DirectionalLight1.DiffuseColor = new Vector3(0.7f);
                    effect.DirectionalLight2.DiffuseColor = new Vector3(0.7f);
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight1.Direction = new Vector3(1, 0, 0);
                    effect.DirectionalLight2.Direction = new Vector3(0, 0, 1);
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight1.Enabled = true;
                    effect.DirectionalLight2.Enabled = true;
                }
            }
        }

        Core.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        m.Draw(
            Matrix.CreateFromYawPitchRoll(cameraRotation.X, cameraRotation.Y, cameraRotation.Z)
                * Matrix.CreateScale(new Vector3(_scale))
                * Matrix.CreateTranslation(modelPosition),
            Matrix.CreateLookAt(cameraPosition, modelPosition, Vector3.Up),
            Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), Core.GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000.0f));

        Core.GraphicsDevice.SamplerStates[0] = Core.Sampler;
        Core.GraphicsDevice.SetRenderTarget(null);
        return renderTarget;
    }
}
