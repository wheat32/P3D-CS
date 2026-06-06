namespace P3D.Servers;

// TODO Phase 8: full ServersManager port
public class ServersManager
{
    public ServerConnection ServerConnection { get; } = new ServerConnection();

    public void Update() { }
}

public class ServerConnection
{
    public bool Connected;

    public void Abort() { }
}
