namespace P3D;

// TODO Phase 9: full NetworkPlayer port
public class NetworkPlayer : Entity
{
    public static void ScreenRegionChanged() { }
    public static String GetTexturePath(String skin) => "Textures\\NPC\\" + skin;
}

// TODO Phase 9: full OnlineStatus port
public static class OnlineStatus
{
    public static void Draw() { }
}

// TODO Phase 9: full Chat port
public static class Chat
{
    public class ChatMessage
    {
        public enum MessageTypes
        {
            Normal,
            CommandMessage,
            ServerMessage,
        }

        public ChatMessage(String sender, String message, String id, MessageTypes type) { }
    }

    public static void AddLine(ChatMessage message) { }
    public static void ClearChat() { }
}
