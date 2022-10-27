using DilmerGames.Core.Singletons;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// This class is responsabile of the synchroniziation of API messages between the server and the clients
/// </summary>

[RequireComponent(typeof(NetworkObject))] 
public class ApiManager: NetworkSingleton<ApiManager>
{ 
    private static NetworkVariable<NetworkString> ApiMessage = new NetworkVariable<NetworkString>("");
    public static string ApiMessageString
    { 
        get
        {
            return ApiMessage.Value;
        }
        set
        {
            ApiMessage.Value = value;
        }
    }

    //private void Start()
    //{

    //}

}

