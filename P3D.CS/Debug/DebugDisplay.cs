using Microsoft.Xna.Framework;

namespace P3D;

public static class DebugDisplay
{
    private static int _drawnVertices;
    private static int _maxVertices;
    private static int _maxVisibleVertices;
    private static int _maxDistance;

    public static int DrawnVertices { get => _drawnVertices; set => _drawnVertices = value; }
    public static int MaxVisibleVertices { get => _maxVisibleVertices; set => _maxVisibleVertices = value; }
    public static int MaxVertices { get => _maxVertices; set => _maxVertices = value; }
    public static int MaxDistance { get => _maxDistance; set => _maxDistance = value; }

    public static void Draw()
    {
        if (Core.CurrentScreen.CanDrawDebug == false || FontManager.MainFont == null)
        {
            return;
        }

        String debugSuffix = String.Empty;
        if (GameController.IS_DEBUG_ACTIVE == true)
        {
            String lastWrite = File.GetLastWriteTime(
                System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
            debugSuffix = $" (Debugmode / {lastWrite})";
        }

        bool actionscriptActive = true;
        if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            actionscriptActive = ((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady;
        }

        String cameraInfo = String.Empty;
        if (Screen.Camera != null)
        {
            String thirdPersonStr = String.Empty;
            if (Screen.Camera.Name.Equals("Overworld") && Screen.Camera is OverworldCamera oc)
            {
                if (oc.ThirdPerson == true)
                {
                    thirdPersonStr = " / " + oc.ThirdPersonOffset;
                }
            }

            String camDir = Screen.Camera.GetFacingDirection() switch
            {
                0 => "C: 0/n",
                1 => "C: 1/w",
                2 => "C: 2/s",
                3 => "C: 3/e",
                _ => ""
            };
            String compassText = thirdPersonStr.Equals("") == false
                ? $"Compass: {camDir}  P: {Screen.Camera.GetPlayerFacingDirection()}"
                : $"Compass: {camDir}";

            cameraInfo = $"{Screen.Camera.Position}{thirdPersonStr}{Environment.NewLine}" +
                         $"{Screen.Camera.Yaw}; {Screen.Camera.Pitch}{Environment.NewLine}" +
                         $"{compassText}{Environment.NewLine}";
        }

        String mapPath = Screen.Level != null
            ? $"{Environment.NewLine}MapPath: {Screen.Level.LevelFile}"
            : "";

        String text =
            $"{GameController.GAMENAME} {GameController.GAMEDEVELOPMENTSTAGE} {GameController.GAMEVERSION}" +
            $" / FPS: {Math.Round(Core.GameInstance.FPSMonitor.Value, 0)}{debugSuffix}{Environment.NewLine}" +
            cameraInfo +
            $"E: {_drawnVertices}/{_maxVertices} ({_maxVisibleVertices}){Environment.NewLine}" +
            $"C: {_maxDistance} A: {actionscriptActive}" +
            mapPath;

        if (Core.GameOptions.ContentPackNames.Length > 0)
        {
            text += Environment.NewLine + "Loaded ContentPacks: " +
                    String.Join(", ", Core.GameOptions.ContentPackNames);
        }

        Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, text,
            new Vector2(7, 7), Color.Black);
        Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, text,
            new Vector2(5, 5), Color.White);
    }
}
