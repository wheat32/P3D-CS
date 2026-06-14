using Microsoft.Xna.Framework.Graphics;

namespace P3D.Resources.Blur;

public class BlurHandler : IDisposable
{
    private const int BLUR_RADIUS = 7;
    private const float BLUR_AMOUNT = 2.0f;

    private GaussianBlur? _blurCore;
    private SpriteBatch? _batch;
    private RenderTarget2D? _rt1;
    private RenderTarget2D? _rt2;

    private bool _isDisposed = false;

    public bool IsDisposed
    {
        get => _isDisposed;
        private set => _isDisposed = value;
    }

    public BlurHandler(int width, int height)
    {
        Effect? fx = null;
        try
        {
            fx = Core.Content.Load<Effect>("Effects/GaussianBlur");
        }
        catch (Microsoft.Xna.Framework.Content.ContentLoadException)
        {
            // Shader not compiled yet (Phase 11). Run unblurred.
            return;
        }
        Init(fx, new SpriteBatch(Core.GraphicsDevice), width, height);
    }

    public BlurHandler(Effect gaussianBlurEffect, SpriteBatch batch, int width, int height) =>
        Init(gaussianBlurEffect, batch, width, height);

    private void Init(Effect gaussianBlurEffect, SpriteBatch batch, int width, int height)
    {
        _batch = batch;
        _blurCore = new GaussianBlur(gaussianBlurEffect);
        _blurCore.ComputeKernel(BLUR_RADIUS, BLUR_AMOUNT);

        int renderTargetWidth = width / 2;
        int renderTargetHeight = height / 2;

        _rt1 = new RenderTarget2D(Core.GraphicsDevice, renderTargetWidth, renderTargetHeight, false,
            Core.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
        _rt2 = new RenderTarget2D(Core.GraphicsDevice, renderTargetWidth, renderTargetHeight, false,
            Core.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);

        _blurCore.ComputeOffsets(renderTargetWidth, renderTargetHeight);
    }

    public Texture2D Perform(Texture2D texture)
    {
        if (_blurCore == null || _rt1 == null || _rt2 == null)
        {
            return texture;
        }
        return _blurCore.PerformGaussianBlur(texture, _rt1, _rt2);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BlurHandler() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (IsDisposed == false)
        {
            if (disposing == true)
            {
                if (_rt1 != null && _rt1.IsDisposed == false) _rt1.Dispose();
                if (_rt2 != null && _rt2.IsDisposed == false) _rt2.Dispose();
                if (_blurCore != null && _blurCore.IsDisposed == false) _blurCore.Dispose();
                if (_batch != null && _batch.IsDisposed == false) _batch.Dispose();
            }
            _rt1 = null!;
            _rt2 = null!;
            _blurCore = null!;
            _batch = null!;
            IsDisposed = true;
        }
    }
}
