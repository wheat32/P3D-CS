using System.Collections;

namespace P3D.Servers;

// TODO Phase 8: full ServersManager port
public class ServersManager
{
    public ServerConnection ServerConnection { get; } = new ServerConnection();
    public PlayerManager PlayerManager { get; } = new PlayerManager();
    public PlayerCollection PlayerCollection { get; } = new PlayerCollection();
    public int ID { get; set; }

    public void Update() { }
}

public class PlayerCollection : IEnumerable<Player>
{
    private List<Player> _players = [];

    public bool HasPlayer(int id) => false;
    public ServerPlayerInfo GetPlayer(int id) => new ServerPlayerInfo();

    public IEnumerator<Player> GetEnumerator() => _players.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _players.GetEnumerator();
}

public class Player
{
    public int ServersID { get; set; }
    public String Name { get; set; } = String.Empty;
}

public class ServerPlayerInfo
{
    public int GameJoltId { get; set; }
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
    public void Disconnect() { }
    public void SendPackage(Package package) { }
    public void SendGameStateMessage(String message) { }
}

public class Package
{
    public enum PackageTypes
    {
        BattleClientData,
        BattlePokemonData,
        BattleHostData,
    }

    public enum ProtocolTypes
    {
        TCP,
        UDP,
    }

    public Package(PackageTypes packageType, int senderID, ProtocolTypes protocol,
                   List<String> data)
    {
    }
}
