using DilmerGames.Core.Singletons;
using Unity.Netcode;

//PlayerManager need a network object, and the reason for that is PlayerManager is an instance inhirts NetworkBehaviour,
//and any object that will have a network variable, have to have a network Object so that the information can be sinchronized
public class PlayersManager : NetworkSingleton<PlayersManager>
{

    NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer || IsHost)
            {
                playersInGame.Value++;
                Logger.Instance.LogInfo($"playerInGame: {playersInGame.Value}");
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer || IsHost)
            {
                playersInGame.Value--;
            }
        };

    }
    
}
