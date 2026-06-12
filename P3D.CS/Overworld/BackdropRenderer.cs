using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace P3D;

public class BackdropRenderer
{
    private List<Backdrop> _backdrops = [];

    public void Initialize()
    {
    }

    public void Clear()
    {
        _backdrops.Clear();
    }

    public void AddBackdrop(Backdrop backdrop)
    {
        _backdrops.Add(backdrop);
    }

    public void Update()
    {
        foreach (Backdrop b in _backdrops)
        {
            b.Update();
        }
    }

    public void Draw()
    {
        RasterizerState tempRasterizer = Core.GraphicsDevice.RasterizerState;

        Core.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        Core.GraphicsDevice.SamplerStates[0] = new SamplerState
        {
            Filter = TextureFilter.Point,
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
        };

        foreach (Backdrop b in _backdrops)
        {
            b.Draw(new short[] { 0, 1, 3, 2, 3, 0 });
        }

        Core.GraphicsDevice.RasterizerState = tempRasterizer;
        Core.GraphicsDevice.SamplerStates[0] = Core.Sampler;
    }

    public class Backdrop
    {
        private const int DEFAULT_FRAME_COUNT = 3;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VertexPositionNormalTangentTexture : IVertexType
        {
            public Vector3 pos;
            public Vector2 uv;
            public Vector3 normal;
            public Vector3 tangent;

            private static readonly VertexElement[] VERTEX_ELEMENTS = [
                new VertexElement(0,  VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
            ];

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(VERTEX_ELEMENTS);

            VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

            public VertexPositionNormalTangentTexture(Vector3 position, Vector3 nor, Vector3 tan, Vector2 texturePosition)
            {
                pos = position;
                normal = nor;
                tangent = tan;
                uv = texturePosition;
            }

            public static int SizeInBytes() => 4 * (3 + 2 + 3 + 3);
        }

        public enum BackdropTypes
        {
            Water,
            Grass,
            Texture,
            Animation,
        }

        private List<VertexPositionNormalTangentTexture> _vertices = [];
        private BackdropTypes _backdropType = BackdropTypes.Grass;
        private Texture2D? _backdropTexture;
        private Texture2D? _customTexture;
        private Matrix _worldMatrix = Matrix.Identity;
        private Vector3 _position;
        private Vector3 _rotation;
        private Effect? _shader;
        private int _width;
        private int _height;

        private Animation? _waterAnimation;
        private Animation? _customAnimation;

        private float _waterAnimationDelay;
        private int _waterAnimationIndex;

        private bool _setTexture;

        public Backdrop(String backdropType, Vector3 position, Vector3 rotation,
                        int width, int height)
            : this(backdropType, position, rotation, width, height, null)
        {
        }

        public Backdrop(String backdropType, Vector3 position, Vector3 rotation,
                        int width, int height, Texture2D? backdropTexture,
                        int animationSpeed = 0, int frameCount = 0)
        {
            _shader = Core.Content.Load<Effect>(@"Effects\BackdropShader");

            _vertices.Add(new VertexPositionNormalTangentTexture(new Vector3(0, 0, 0),      new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
            _vertices.Add(new VertexPositionNormalTangentTexture(new Vector3(width, 0, 0),  new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector2(1, 0)));
            _vertices.Add(new VertexPositionNormalTangentTexture(new Vector3(0, 0, height), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
            _vertices.Add(new VertexPositionNormalTangentTexture(new Vector3(width, 0, height), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector2(1, 1)));

            _position = position;
            _rotation = rotation;
            _backdropTexture = backdropTexture;
            _width = width;
            _height = height;

            switch (backdropType.ToLower())
            {
                case "water":
                    _backdropType = BackdropTypes.Water;
                    _waterAnimationDelay = 1f / GameModeManager.ActiveGameMode.WaterSpeed;
                    Texture2D waterTex = TextureManager.GetTexture(@"Textures\Backdrops\Water");
                    Size waterSize = new Size((int)(waterTex.Width / DEFAULT_FRAME_COUNT), (int)waterTex.Height);
                    _waterAnimation = new Animation(waterTex, 1, DEFAULT_FRAME_COUNT, waterSize.Width, waterSize.Height, GameModeManager.ActiveGameMode.WaterSpeed, 0, 0);
                    _backdropTexture = TextureManager.GetTexture(@"Textures\Backdrops\Water", _waterAnimation.TextureRectangle, String.Empty);
                    break;
                case "grass":
                    _backdropType = BackdropTypes.Grass;
                    break;
                case "texture":
                    _backdropType = BackdropTypes.Texture;
                    break;
                case "animation":
                {
                    _backdropType = BackdropTypes.Animation;
                    _customTexture = backdropTexture;
                    int actualAnimSpeed = animationSpeed == 0 ? GameModeManager.ActiveGameMode.WaterSpeed : animationSpeed;
                    int actualFrameCount = frameCount == 0 ? DEFAULT_FRAME_COUNT : frameCount;
                    Size animSize = new Size((int)(_customTexture!.Width / actualFrameCount), (int)_customTexture.Height);
                    _customAnimation = new Animation(_customTexture, 1, actualFrameCount, animSize.Width, animSize.Height, actualAnimSpeed, 0, 0);
                    _backdropTexture = TextureManager.GetTexture(_customTexture, _customAnimation.TextureRectangle);
                    break;
                }
            }

            Update();
        }

        public void Update()
        {
            _worldMatrix = Matrix.CreateFromYawPitchRoll(_rotation.Y, _rotation.X, _rotation.Z) * Matrix.CreateTranslation(_position);

            switch (_backdropType)
            {
                case BackdropTypes.Water:
                    if (Core.GameOptions.GraphicStyle == 1)
                    {
                        if (_waterAnimation != null)
                        {
                            _waterAnimation.Update(0.01f);
                            _backdropTexture = TextureManager.GetTexture(@"Textures\Backdrops\Water", _waterAnimation.TextureRectangle, String.Empty);
                        }
                    }
                    break;
                case BackdropTypes.Grass:
                    if (_setTexture == false)
                    {
                        Texture2D grassTex = TextureManager.GetTexture(@"Textures\Backdrops\Grass");
                        Size grassSize = new Size((int)(grassTex.Width / 4), (int)grassTex.Height);
                        int x = 0;

                        switch (World.CurrentSeason)
                        {
                            case World.Seasons.Winter:
                                x = 0;
                                break;
                            case World.Seasons.Spring:
                                x = grassSize.Width;
                                break;
                            case World.Seasons.Summer:
                                x = grassSize.Width * 2;
                                break;
                            case World.Seasons.Fall:
                                x = grassSize.Width * 3;
                                break;
                        }

                        _backdropTexture = TextureManager.GetTexture(@"Backdrops\Grass", new Rectangle(x, 0, grassSize.Width, grassSize.Height), String.Empty);
                        _setTexture = true;
                    }
                    break;
                case BackdropTypes.Animation:
                    if (Core.GameOptions.GraphicStyle == 1)
                    {
                        if (_customAnimation != null)
                        {
                            _customAnimation.Update(0.005f);
                            _backdropTexture = TextureManager.GetTexture(_customTexture!, _customAnimation.TextureRectangle);
                        }
                    }
                    else
                    {
                        if (_setTexture == false)
                        {
                            _backdropTexture = TextureManager.GetTexture(_customTexture!, _customAnimation!.TextureRectangle);
                            _setTexture = true;
                        }
                    }
                    break;
            }
        }

        public void Draw(short[] indices)
        {
            if (_backdropTexture == null || _shader == null) return;

            VertexBuffer vBuffer = new VertexBuffer(
                Core.GraphicsDevice,
                VertexPositionNormalTangentTexture.VertexDeclaration,
                _vertices.Count,
                BufferUsage.None);
            IndexBuffer iBuffer = new IndexBuffer(
                Core.GraphicsDevice,
                typeof(short),
                indices.Length,
                BufferUsage.None);

            vBuffer.SetData(_vertices.ToArray());
            iBuffer.SetData(indices);

            _shader.Parameters["World"].SetValue(_worldMatrix);
            _shader.CurrentTechnique = _shader.Techniques["Texture"];
            _shader.Parameters["View"].SetValue(Screen.Camera!.View);
            _shader.Parameters["Projection"].SetValue(Screen.Camera.Projection);
            _shader.Parameters["DiffuseColor"].SetValue(GetDiffuseColor());
            _shader.Parameters["TexStretch"].SetValue(new Vector2(_width, _height));
            _shader.Parameters["color"].SetValue(_backdropTexture);

            foreach (EffectPass pass in _shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                Core.GraphicsDevice.SetVertexBuffer(vBuffer);
                Core.GraphicsDevice.Indices = iBuffer;
                Core.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Count);
            }

            vBuffer.Dispose();
            iBuffer.Dispose();
        }

        private Vector4 GetDiffuseColor()
        {
            Vector3 dayColor = Vector3.One;
            Vector3 diffuseColor = Screen.Effect != null ? Screen.Effect.DiffuseColor : Vector3.One;

            if (Screen.Level?.World != null)
            {
                switch (Screen.Level.World.EnvironmentType)
                {
                    case World.EnvironmentTypes.Outside:
                        if (Core.GameOptions.LightingEnabled == true)
                        {
                            dayColor = SkyDome.GetDaytimeColor(true).ToVector3();
                        }
                        break;
                    case World.EnvironmentTypes.Dark:
                        dayColor = new Vector3(0.5f, 0.5f, 0.5f);
                        break;
                }
            }

            if (Core.GameOptions.LightingEnabled == true)
            {
                return (dayColor * diffuseColor * Lighting.GetEnvironmentColor(1)).ToColor().ToVector4();
            }
            else
            {
                return (dayColor * diffuseColor).ToColor().ToVector4();
            }
        }
    }
}
