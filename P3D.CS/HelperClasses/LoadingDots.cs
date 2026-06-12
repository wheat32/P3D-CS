namespace P3D;

public static class LoadingDots
{
    private const float DELAY_INCREMENT = 0.1f;
    private const float DELAY_MAX = 4.0f;
    private const float THRESHOLD_ONE_DOT = 1.0f;
    private const float THRESHOLD_TWO_DOTS = 2.0f;
    private const float THRESHOLD_THREE_DOTS = 3.0f;

    private static float _pointsDelay;

    public static void Update()
    {
        _pointsDelay += DELAY_INCREMENT;
        if (_pointsDelay >= DELAY_MAX)
        {
            _pointsDelay = 0f;
        }
    }

    public static String Dots
    {
        get
        {
            String p = String.Empty;
            if (_pointsDelay >= THRESHOLD_ONE_DOT)
            {
                p += ".";
            }
            if (_pointsDelay >= THRESHOLD_TWO_DOTS)
            {
                p += ".";
            }
            if (_pointsDelay >= THRESHOLD_THREE_DOTS)
            {
                p += ".";
            }
            return p;
        }
    }
}
