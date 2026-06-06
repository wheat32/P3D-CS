namespace P3D;

public class PlayerTemp
{
    private const int DEFAULT_DAYCARE_CYCLE = 256;

    public int DayCareCycle { get; set; } = DEFAULT_DAYCARE_CYCLE;

    public PlayerTemp()
    {
        Reset();
    }

    public void Reset()
    {
        DayCareCycle = DEFAULT_DAYCARE_CYCLE;
    }
}
