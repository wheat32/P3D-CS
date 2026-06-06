namespace P3D;

// TODO Phase 2: full ObjectDump port
public class ObjectDump
{
    public String Dump { get; }

    public ObjectDump(Screen? screen)
    {
        Dump = screen != null
            ? $"Screen: {screen.Identification}"
            : "[No screen]";
    }
}
