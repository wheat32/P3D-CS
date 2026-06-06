using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class Animation : BasicObject
{
    public enum PlayMode
    {
        Playing,
        Stopped,
        Paused
    }

    private float _totalElapsed;
    private PlayMode _running = PlayMode.Playing;

    public float TotalElapsed
    {
        get => _totalElapsed;
        set => _totalElapsed = value;
    }

    public int Rows { get; set; }
    public int Columns { get; set; }
    public float AnimationSpeed { get; set; }
    public int CurrentRow { get; set; }
    public int CurrentColumn { get; set; }
    public int StartRow { get; set; }
    public int StartColumn { get; set; }

    public Rectangle TextureRectangle =>
        new Rectangle(CurrentColumn * Width, CurrentRow * Height, Width, Height);

    public PlayMode Running => _running;

    public Animation(Texture2D texture, int rows, int columns, int width, int height,
                     int animationSpeed, int startRow, int startColumn)
        : base(texture, width, height, Vector2.Zero)
    {
        Rows = rows;
        Columns = columns;
        AnimationSpeed = 1f / animationSpeed;
        _totalElapsed = 0;
        CurrentRow = startRow;
        CurrentColumn = startColumn;
        StartRow = startRow;
        StartColumn = startColumn;
    }

    public void Update(float elapsed)
    {
        if (_running == PlayMode.Playing)
        {
            _totalElapsed += elapsed;
            if (_totalElapsed > AnimationSpeed)
            {
                _totalElapsed -= AnimationSpeed;
                CurrentColumn++;
                if (CurrentColumn >= Columns)
                {
                    CurrentRow++;
                    CurrentColumn = 0;
                    if (CurrentRow >= Rows)
                    {
                        CurrentRow = 0;
                    }
                }
            }
        }
    }

    public void Start()
    {
        _running = PlayMode.Playing;
    }

    public void Stop()
    {
        _running = PlayMode.Stopped;
        CurrentRow = StartRow;
        CurrentColumn = StartColumn;
    }

    public void Restart()
    {
        _running = PlayMode.Playing;
        CurrentRow = StartRow;
        CurrentColumn = StartColumn;
    }

    public void Pause()
    {
        _running = PlayMode.Paused;
    }
}
