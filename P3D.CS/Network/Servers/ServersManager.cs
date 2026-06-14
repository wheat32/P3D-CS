using System.Collections;

namespace P3D.Servers;

// TODO Phase 9: full ServersManager port
public class ServersManager
{
    public const String PROTOCOLVERSION = "0.5";

    public ServerConnection ServerConnection { get; } = new ServerConnection();
    public PlayerManager PlayerManager { get; } = new PlayerManager();
    public PlayerCollection PlayerCollection { get; } = new PlayerCollection();
    public int ID { get; set; }

    public void Update() { }
    public void Connect(Object server) { }
}

public class Server
{
    public String IP = String.Empty;
    public String Port = String.Empty;

    public Server(String address)
    {
        if (address.Contains(":") == true)
        {
            IP = address.Split(':')[0];
            Port = address.Split(':')[1];
        }
        else
        {
            IP = address;
            Port = "15124";
        }
    }
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
    public String GameJoltId { get; set; } = String.Empty;
    public String Skin { get; set; } = String.Empty;
}

public class ServerPlayerInfo
{
    public int GameJoltId { get; set; }
}

public class PlayerManager
{
    public bool NeedsUpdate { get; set; }
    public void UpdatePlayers() { }
    public bool ReceivedIniData() => false;
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
