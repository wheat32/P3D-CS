using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Resources.Blur;

public class GaussianBlur : IDisposable
{
    private Effect _effect;
    private SpriteBatch _spriteBatch;
    private int _radius;
    private float _amount;
    private float _sigma;
    private float[] _kernel = [];
    private Vector2[] _offsetHoriz = [];
    private Vector2[] _offsetVert = [];

    private bool _isDisposed = false;

    public bool IsDisposed
    {
        get => _isDisposed;
        private set => _isDisposed = value;
    }

    public GaussianBlur(Effect gaussianBlurEffect)
    {
        _effect = gaussianBlurEffect;
        _spriteBatch = new SpriteBatch(Core.GraphicsDevice);
    }

    internal void ComputeKernel(int blurRadius, float blurAmount)
    {
        _radius = blurRadius;
        _amount = blurAmount;

        _kernel = new float[_radius * 2 + 1];
        _sigma = _radius / _amount;

        float twoSigmaSquare = 2.0f * _sigma * _sigma;
        float sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
        float total = 0f;

        for (int i = -_radius; i <= _radius; i++)
        {
            float distance = i * i;
            int index = i + _radius;
            _kernel[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
            total += _kernel[index];
        }

        for (int i = 0; i < _kernel.Length; i++)
            _kernel[i] /= total;
    }

    internal void ComputeOffsets(float textureWidth, float textureHeight)
    {
        _offsetHoriz = new Vector2[_radius * 2 + 1];
        _offsetVert = new Vector2[_radius * 2 + 1];

        float xOffset = 1.0f / textureWidth;
        float yOffset = 1.0f / textureHeight;

        for (int i = -_radius; i <= _radius; i++)
        {
            int index = i + _radius;
            _offsetHoriz[index] = new Vector2(i * xOffset, 0f);
            _offsetVert[index] = new Vector2(0f, i * yOffset);
        }
    }

    internal Texture2D PerformGaussianBlur(Texture2D srcTexture, RenderTarget2D renderTarget1, RenderTarget2D renderTarget2)
    {
        if (_effect == null)
            throw new InvalidOperationException("Blur effect not loaded");

        Rectangle srcRect = new Rectangle(0, 0, srcTexture.Width, srcTexture.Height);
        Rectangle destRect1 = new Rectangle(0, 0, renderTarget1.Width, renderTarget1.Height);
        Rectangle destRect2 = new Rectangle(0, 0, renderTarget2.Width, renderTarget2.Height);

        Core.GraphicsDevice.SetRenderTarget(renderTarget1);
        _effect.CurrentTechnique = _effect.Techniques["GaussianBlur"];
        _effect.Parameters["weights"].SetValue(_kernel);
        _effect.Parameters["colorMapTexture"].SetValue(srcTexture);
        _effect.Parameters["offsets"].SetValue(_offsetHoriz);

        _spriteBatch.Begin(effect: _effect);
        _spriteBatch.Draw(srcTexture, destRect1, Color.White);
        _spriteBatch.End();

        Core.GraphicsDevice.SetRenderTarget(renderTarget2);
        Texture2D outputTexture = renderTarget1;

        _effect.Parameters["colorMapTexture"].SetValue(outputTexture);
        _effect.Parameters["offsets"].SetValue(_offsetVert);

        _spriteBatch.Begin(effect: _effect);
        _spriteBatch.Draw(outputTexture, destRect2, Color.White);
        _spriteBatch.End();

        Core.GraphicsDevice.SetRenderTarget(null);
        return renderTarget2;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~GaussianBlur() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (IsDisposed == false)
        {
            if (disposing == true)
            {
                if (_spriteBatch != null && _spriteBatch.IsDisposed == false)
                    _spriteBatch.Dispose();
            }
            _effect = null!;
            _spriteBatch = null!;
            IsDisposed = true;
        }
    }
}
