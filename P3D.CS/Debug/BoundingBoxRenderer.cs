using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class BoundingBoxRenderer
{
    private const int CORNER_COUNT = 8;
    private static readonly short[] INDICES =
    [
        0, 1, 1, 2, 2, 3, 3, 0,
        0, 4, 1, 5, 2, 6, 3, 7,
        4, 5, 5, 6, 6, 7, 7, 4
    ];

    private static VertexPositionColor[] _verts = new VertexPositionColor[CORNER_COUNT];
    private static BasicEffect? _effect;

    private static void EnsureEffect(GraphicsDevice graphicsDevice)
    {
        if (_effect != null)
        {
            return;
        }
        _effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };
    }

    public static void Render(BoundingBox box, GraphicsDevice graphicsDevice,
                              Matrix view, Matrix projection, Color color)
    {
        EnsureEffect(graphicsDevice);
        Vector3[] corners = box.GetCorners();
        for (int i = 0; i < CORNER_COUNT; i++)
        {
            _verts[i].Position = corners[i];
            _verts[i].Color = color;
        }
        _effect!.View = view;
        _effect.Projection = projection;
        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineList, _verts, 0, CORNER_COUNT,
                INDICES, 0, INDICES.Length / 2);
        }
    }

    public static void Render(BoundingFrustum frustum, GraphicsDevice graphicsDevice,
                              Matrix view, Matrix projection, Color color)
    {
        EnsureEffect(graphicsDevice);
        Vector3[] corners = frustum.GetCorners();
        for (int i = 0; i < CORNER_COUNT; i++)
        {
            _verts[i].Position = corners[i];
            _verts[i].Color = color;
        }
        _effect!.View = view;
        _effect.Projection = projection;
        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineList, _verts, 0, CORNER_COUNT,
                INDICES, 0, INDICES.Length / 2);
        }
    }
}
