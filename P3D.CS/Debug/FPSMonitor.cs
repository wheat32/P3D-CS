using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace P3D;

public class FPSMonitor
{
    private const int SAMPLE_MILLISECONDS = 100;

    public double Value { get; private set; }
    public TimeSpan Sample { get; set; }

    private readonly Stopwatch _sw;
    private int _frames;

    public FPSMonitor()
    {
        Sample = TimeSpan.FromMilliseconds(SAMPLE_MILLISECONDS);
        Value = 0;
        _frames = 0;
        _sw = Stopwatch.StartNew();
    }

    public void Update(GameTime gameTime)
    {
        if (_sw.Elapsed > Sample)
        {
            Value = _frames / _sw.Elapsed.TotalSeconds;
            _sw.Reset();
            _sw.Start();
            _frames = 0;
        }
    }

    public void DrawnFrame()
    {
        _frames++;
    }
}
