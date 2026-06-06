namespace P3D.Servers;

// TODO Phase 8: full ServersManager port
public class ServersManager
{
    public ServerConnection ServerConnection { get; } = new ServerConnection();
    public PlayerManager PlayerManager { get; } = new PlayerManager();

    public void Update() { }
}

public class PlayerManager
{
    public bool NeedsUpdate { get; set; }
    public void UpdatePlayers() { }
}

public class ServerConnection
{
    public bool Connected;

    public void Abort() { }
}
