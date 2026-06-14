namespace P3D;

public class HistoryScreen
{
    public static class HistoryHandler
    {
        public static void AddHistoryItem(String name, String data, bool isScriptOrigin, bool showOnTimeline)
        {
            if (Core.Player.HistoryData != String.Empty)
                Core.Player.HistoryData += Environment.NewLine;

            DateTime now = DateTime.Now;
            String hour = now.Hour.ToString().PadLeft(2, '0');
            String minute = now.Minute.ToString().PadLeft(2, '0');
            String second = now.Second.ToString().PadLeft(2, '0');
            String day = now.Day.ToString().PadLeft(2, '0');
            String month = now.Month.ToString().PadLeft(2, '0');
            String year = now.Year.ToString();

            String dateString = day + "-" + month + "-" + year + "_" + hour + "." + minute + "." + second;

            Core.Player.HistoryData += dateString + "|" + isScriptOrigin.ToNumberString() + "|" + name + "|" + data + "|" + showOnTimeline.ToNumberString();
        }
    }
}
