using Microsoft.Xna.Framework;

namespace P3D;

public class ChooseBox
{
    public delegate void DoAnswer(int result);

    public String[] Options = [];
    public int Index;
    public bool Showing;
    public bool ReadyForResult;
    public int Result;
    public int ResultID;
    public bool ActionScript;
    public static int CancelIndex = -1;
    public FontContainer? TextFont;
    public bool DoDelegate;
    public Entity[]? UpdateEntities;

    private DoAnswer? _subs;
    private float _positionY;

    public void Show(String[] options, DoAnswer doSubs)
    {
        ResultID = 0;
        Options = options;
        Index = 0;
        ReadyForResult = false;
        Showing = true;
        _subs = doSubs;
        _positionY = Core.windowSize.Height;
    }

    public void Show(String[] options, int defaultIndex, Entity[] entities)
    {
        ResultID = 0;
        Options = options;
        Index = defaultIndex;
        ReadyForResult = false;
        Showing = true;
        UpdateEntities = entities;
        _positionY = Core.windowSize.Height;
    }

    public void Update() { }
    public void Draw() { }
}
