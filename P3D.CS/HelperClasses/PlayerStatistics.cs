namespace P3D;

public static class PlayerStatistics
{
    private static readonly Dictionary<String, int> _statistics = [];

    public static void Load(String data)
    {
        _statistics.Clear();
        foreach (String line in data.SplitAtNewline())
        {
            if (line.Contains(",") == false)
            {
                continue;
            }

            String statName = line[..line.IndexOf(',')];
            int statValue = int.Parse(line[(line.IndexOf(',') + 1)..]);

            _statistics[statName] = statValue;
        }
    }

    public static void Track(String statName, int addition)
    {
        if (_statistics.ContainsKey(statName) == true)
        {
            _statistics[statName] += addition;
        }
        else
        {
            _statistics.Add(statName, addition);
        }

        if (GameJolt.API.LoggedIn == true)
        {
            GameJolt.GameJoltStatistics.Track(statName, addition);
        }
    }

    public static String GetData()
    {
        IEnumerable<String> lines = _statistics.Select(kv => kv.Key + "," + kv.Value);
        return String.Join(Environment.NewLine, lines);
    }

    public static int CountStatistics()
    {
        return _statistics.Count;
    }
}
