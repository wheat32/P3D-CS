using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// Shader file: Content/Effects/BasicEffectWithAlphaTest.mgfxogl (produced by Phase 7 shader migration task)
public sealed class BasicEffectWithAlphaTest : Effect, IEffectMatrices, IEffectLights, IEffectFog
{
    public DirectionalLight DirectionalLight0 { get; private set; } = null!;
    public DirectionalLight DirectionalLight1 { get; private set; } = null!;
    public DirectionalLight DirectionalLight2 { get; private set; } = null!;

    public Matrix World
    {
        get => _world;
        set
        {
            _world = value;
            _dirtyFlags |= EffectDirtyFlags.World | EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
        }
    }

    public Matrix View
    {
        get => _view;
        set
        {
            _view = value;
            _dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.EyePosition | EffectDirtyFlags.Fog;
        }
    }

    public Matrix Projection
    {
        get => _projection;
        set
        {
            _projection = value;
            _dirtyFlags |= EffectDirtyFlags.WorldViewProj;
        }
    }

    public Vector3 DiffuseColor
    {
        get => _diffuseColor;
        set
        {
            _diffuseColor = value;
            _dirtyFlags |= EffectDirtyFlags.MaterialColor;
        }
    }

    public Vector3 EmissiveColor
    {
        get => _emissiveColor;
        set
        {
            _emissiveColor = value;
            _dirtyFlags |= EffectDirtyFlags.MaterialColor;
        }
    }

    public Vector3 SpecularColor
    {
        get => _specularColorParam.GetValueVector3();
        set => _specularColorParam.SetValue(value);
    }

    public float SpecularPower
    {
        get => _specularPowerParam.GetValueSingle();
        set => _specularPowerParam.SetValue(value);
    }

    public float Alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            _dirtyFlags |= EffectDirtyFlags.MaterialColor;
        }
    }

    public bool LightingEnabled
    {
        get => _lightingEnabled;
        set
        {
            if (_lightingEnabled != value)
            {
                _lightingEnabled = value;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex | EffectDirtyFlags.MaterialColor;
            }
        }
    }

    public bool PreferPerPixelLighting
    {
        get => _preferPerPixelLighting;
        set
        {
            if (_preferPerPixelLighting != value)
            {
                _preferPerPixelLighting = value;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }
        }
    }

    public Vector3 AmbientLightColor
    {
        get => _ambientLightColor;
        set
        {
            _ambientLightColor = value;
            _dirtyFlags |= EffectDirtyFlags.MaterialColor;
        }
    }

    public bool FogEnabled
    {
        get => _fogEnabled;
        set
        {
            if (_fogEnabled != value)
            {
                _fogEnabled = value;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex | EffectDirtyFlags.FogEnable;
            }
        }
    }

    public float FogStart
    {
        get => _fogStart;
        set
        {
            _fogStart = value;
            _dirtyFlags |= EffectDirtyFlags.Fog;
        }
    }

    public float FogEnd
    {
        get => _fogEnd;
        set
        {
            _fogEnd = value;
            _dirtyFlags |= EffectDirtyFlags.Fog;
        }
    }

    public Vector3 FogColor
    {
        get => _fogColorParam.GetValueVector3();
        set => _fogColorParam.SetValue(value);
    }

    public bool TextureEnabled
    {
        get => _textureEnabled;
        set
        {
            if (_textureEnabled != value)
            {
                _textureEnabled = value;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }
        }
    }

    public Texture2D Texture
    {
        get => _textureParam.GetValueTexture2D();
        set => _textureParam.SetValue(value);
    }

    public bool VertexColorEnabled
    {
        get => _vertexColorEnabled;
        set
        {
            if (_vertexColorEnabled != value)
            {
                _vertexColorEnabled = value;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }
        }
    }

    public float AlphaCutoff
    {
        get => _alphaTestParam.GetValueSingle();
        set => _alphaTestParam.SetValue(value);
    }

    public bool EnableHardwareInstancing
    {
        get => _hwEnabled;
        set
        {
            if (_hwEnabled != value)
            {
                _hwEnabled = value;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }
        }
    }

    private EffectParameter _textureParam = null!;
    private EffectParameter _diffuseColorParam = null!;
    private EffectParameter _emissiveColorParam = null!;
    private EffectParameter _specularColorParam = null!;
    private EffectParameter _specularPowerParam = null!;
    private EffectParameter _eyePositionParam = null!;
    private EffectParameter _fogColorParam = null!;
    private EffectParameter _fogVectorParam = null!;
    private EffectParameter _worldParam = null!;
    private EffectParameter _worldInverseTransposeParam = null!;
    private EffectParameter _worldViewProjParam = null!;
    private EffectParameter _alphaTestParam = null!;

    private bool _lightingEnabled;
    private bool _preferPerPixelLighting;
    private bool _oneLight;
    private bool _fogEnabled;
    private bool _textureEnabled;
    private bool _vertexColorEnabled;
    private Matrix _world = Matrix.Identity;
    private Matrix _view = Matrix.Identity;
    private Matrix _projection = Matrix.Identity;
    private Matrix _worldView;
    private Vector3 _diffuseColor = Vector3.One;
    private Vector3 _emissiveColor = Vector3.Zero;
    private Vector3 _ambientLightColor = Vector3.Zero;
    private float _alpha = 1;
    private float _fogStart;
    private float _fogEnd = 1;
    private bool _hwEnabled;
    private EffectDirtyFlags _dirtyFlags = EffectDirtyFlags.All;

    private static byte[] LoadShaderBytes() =>
        File.ReadAllBytes(Path.Combine(
            Path.GetDirectoryName(AppContext.BaseDirectory)!,
            "Content", "Effects", "BasicEffectWithAlphaTest.mgfxogl"));

    public BasicEffectWithAlphaTest(GraphicsDevice device) : base(device, LoadShaderBytes())
    {
        CacheEffectParameters();
        DirectionalLight0.Enabled = true;
        SpecularColor = Vector3.One;
        SpecularPower = 16;
        AlphaCutoff = 0;
    }

    public BasicEffectWithAlphaTest(GraphicsDevice device, byte[] data) : base(device, data)
    {
        CacheEffectParameters();
        DirectionalLight0.Enabled = true;
        SpecularColor = Vector3.One;
        SpecularPower = 16;
        AlphaCutoff = 0;
    }

    public BasicEffectWithAlphaTest(BasicEffectWithAlphaTest cloneSource) : base(cloneSource)
    {
        CacheEffectParameters(cloneSource.DirectionalLight0, cloneSource.DirectionalLight1, cloneSource.DirectionalLight2);
        _lightingEnabled = cloneSource._lightingEnabled;
        _preferPerPixelLighting = cloneSource._preferPerPixelLighting;
        _fogEnabled = cloneSource._fogEnabled;
        _textureEnabled = cloneSource._textureEnabled;
        _vertexColorEnabled = cloneSource._vertexColorEnabled;
        _world = cloneSource._world;
        _view = cloneSource._view;
        _projection = cloneSource._projection;
        _diffuseColor = cloneSource._diffuseColor;
        _emissiveColor = cloneSource._emissiveColor;
        _ambientLightColor = cloneSource._ambientLightColor;
        _alpha = cloneSource._alpha;
        _fogStart = cloneSource._fogStart;
        _fogEnd = cloneSource._fogEnd;
    }

    public BasicEffectWithAlphaTest(BasicEffect cloneSource)
        : base(cloneSource.GraphicsDevice, LoadShaderBytes())
    {
        CacheEffectParameters(cloneSource.DirectionalLight0, cloneSource.DirectionalLight1, cloneSource.DirectionalLight2);
        DirectionalLight0.Direction = cloneSource.DirectionalLight0.Direction;
        DirectionalLight0.Enabled = cloneSource.DirectionalLight0.Enabled;
        DirectionalLight0.DiffuseColor = cloneSource.DirectionalLight0.DiffuseColor;
        DirectionalLight0.SpecularColor = cloneSource.DirectionalLight0.SpecularColor;
        DirectionalLight1.Direction = cloneSource.DirectionalLight1.Direction;
        DirectionalLight1.Enabled = cloneSource.DirectionalLight1.Enabled;
        DirectionalLight1.DiffuseColor = cloneSource.DirectionalLight1.DiffuseColor;
        DirectionalLight1.SpecularColor = cloneSource.DirectionalLight1.SpecularColor;
        DirectionalLight2.Direction = cloneSource.DirectionalLight2.Direction;
        DirectionalLight2.Enabled = cloneSource.DirectionalLight2.Enabled;
        DirectionalLight2.DiffuseColor = cloneSource.DirectionalLight2.DiffuseColor;
        DirectionalLight2.SpecularColor = cloneSource.DirectionalLight2.SpecularColor;
        World = cloneSource.World;
        View = cloneSource.View;
        Projection = cloneSource.Projection;
        DiffuseColor = cloneSource.DiffuseColor;
        EmissiveColor = cloneSource.EmissiveColor;
        SpecularColor = cloneSource.SpecularColor;
        SpecularPower = cloneSource.SpecularPower;
        Alpha = cloneSource.Alpha;
        LightingEnabled = cloneSource.LightingEnabled;
        PreferPerPixelLighting = cloneSource.PreferPerPixelLighting;
        AmbientLightColor = cloneSource.AmbientLightColor;
        FogEnabled = cloneSource.FogEnabled;
        FogStart = cloneSource.FogStart;
        FogEnd = cloneSource.FogEnd;
        FogColor = cloneSource.FogColor;
        TextureEnabled = cloneSource.TextureEnabled;
        Texture = cloneSource.Texture;
        VertexColorEnabled = cloneSource.VertexColorEnabled;
    }

    public static implicit operator BasicEffect(BasicEffectWithAlphaTest effect)
    {
        BasicEffect newEffect = new BasicEffect(effect.GraphicsDevice);
        newEffect.DirectionalLight0.Enabled = effect.DirectionalLight0.Enabled;
        newEffect.DirectionalLight0.Direction = effect.DirectionalLight0.Direction;
        newEffect.DirectionalLight0.DiffuseColor = effect.DirectionalLight0.DiffuseColor;
        newEffect.DirectionalLight0.SpecularColor = effect.DirectionalLight0.SpecularColor;
        newEffect.DirectionalLight1.Enabled = effect.DirectionalLight1.Enabled;
        newEffect.DirectionalLight1.Direction = effect.DirectionalLight1.Direction;
        newEffect.DirectionalLight1.DiffuseColor = effect.DirectionalLight1.DiffuseColor;
        newEffect.DirectionalLight1.SpecularColor = effect.DirectionalLight1.SpecularColor;
        newEffect.DirectionalLight2.Enabled = effect.DirectionalLight2.Enabled;
        newEffect.DirectionalLight2.Direction = effect.DirectionalLight2.Direction;
        newEffect.DirectionalLight2.DiffuseColor = effect.DirectionalLight2.DiffuseColor;
        newEffect.DirectionalLight2.SpecularColor = effect.DirectionalLight2.SpecularColor;
        newEffect.World = effect.World;
        newEffect.View = effect.View;
        newEffect.Projection = effect.Projection;
        newEffect.DiffuseColor = effect.DiffuseColor;
        newEffect.EmissiveColor = effect.EmissiveColor;
        newEffect.SpecularColor = effect.SpecularColor;
        newEffect.SpecularPower = effect.SpecularPower;
        newEffect.Alpha = effect.Alpha;
        newEffect.LightingEnabled = effect.LightingEnabled;
        newEffect.PreferPerPixelLighting = effect.PreferPerPixelLighting;
        newEffect.AmbientLightColor = effect.AmbientLightColor;
        newEffect.FogEnabled = effect.FogEnabled;
        newEffect.FogStart = effect.FogStart;
        newEffect.FogEnd = effect.FogEnd;
        newEffect.FogColor = effect.FogColor;
        newEffect.TextureEnabled = effect.TextureEnabled;
        newEffect.Texture = effect.Texture;
        newEffect.VertexColorEnabled = effect.VertexColorEnabled;
        return newEffect;
    }

    public override Effect Clone() => new BasicEffectWithAlphaTest(this);

    public void EnableDefaultLighting()
    {
        LightingEnabled = true;
        AmbientLightColor = EffectHelpers.EnableDefaultLighting(DirectionalLight0, DirectionalLight1, DirectionalLight2);
    }

    protected override void OnApply()
    {
        _dirtyFlags = EffectHelpers.SetWorldViewProjAndFog(_dirtyFlags,
            ref _world, ref _view, ref _projection, ref _worldView,
            _fogEnabled, _fogStart, _fogEnd, _worldViewProjParam, _fogVectorParam);

        if ((_dirtyFlags & EffectDirtyFlags.MaterialColor) != 0)
        {
            EffectHelpers.SetMaterialColor(_lightingEnabled, _alpha, ref _diffuseColor, ref _emissiveColor, ref _ambientLightColor, _diffuseColorParam, _emissiveColorParam);
            _dirtyFlags &= ~EffectDirtyFlags.MaterialColor;
        }

        if (_lightingEnabled == true)
        {
            _dirtyFlags = EffectHelpers.SetLightingMatrices(_dirtyFlags,
                ref _world, ref _view, _worldParam, _worldInverseTransposeParam, _eyePositionParam);

            bool newOneLight = DirectionalLight1.Enabled == false && DirectionalLight2.Enabled == false;
            if (_oneLight != newOneLight)
            {
                _oneLight = newOneLight;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }
        }

        if ((_dirtyFlags & EffectDirtyFlags.ShaderIndex) != 0)
        {
            int shaderIndex = 0;

            if (_fogEnabled == false)
                shaderIndex += 1;
            if (_vertexColorEnabled == true)
                shaderIndex += 2;
            if (_textureEnabled == true)
                shaderIndex += 4;
            if (_lightingEnabled == true)
            {
                if (_preferPerPixelLighting == true)
                    shaderIndex += 24;
                else if (_oneLight == true)
                    shaderIndex += 16;
                else
                    shaderIndex += 8;
            }
            if (_hwEnabled == true)
                shaderIndex += 32;

            _dirtyFlags &= ~EffectDirtyFlags.ShaderIndex;
            CurrentTechnique = Techniques[shaderIndex];
        }
    }

    private void CacheEffectParameters(DirectionalLight? copyLight0 = null, DirectionalLight? copyLight1 = null, DirectionalLight? copyLight2 = null)
    {
        _textureParam = Parameters["Texture"];
        _diffuseColorParam = Parameters["DiffuseColor"];
        _emissiveColorParam = Parameters["EmissiveColor"];
        _specularColorParam = Parameters["SpecularColor"];
        _specularPowerParam = Parameters["SpecularPower"];
        _eyePositionParam = Parameters["EyePosition"];
        _fogColorParam = Parameters["FogColor"];
        _fogVectorParam = Parameters["FogVector"];
        _worldParam = Parameters["World"];
        _worldInverseTransposeParam = Parameters["WorldInverseTranspose"];
        _worldViewProjParam = Parameters["WorldViewProj"];
        _alphaTestParam = Parameters["AlphaTest"];
        DirectionalLight0 = new DirectionalLight(Parameters["DirLight0Direction"], Parameters["DirLight0DiffuseColor"], Parameters["DirLight0SpecularColor"], copyLight0);
        DirectionalLight1 = new DirectionalLight(Parameters["DirLight1Direction"], Parameters["DirLight1DiffuseColor"], Parameters["DirLight1SpecularColor"], copyLight1);
        DirectionalLight2 = new DirectionalLight(Parameters["DirLight2Direction"], Parameters["DirLight2DiffuseColor"], Parameters["DirLight2SpecularColor"], copyLight2);
    }
}
