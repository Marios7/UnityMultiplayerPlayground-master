using DilmerGames.Core.Singletons;
using Unity.Netcode;
using UnityEngine;

public class SpawnerControl : NetworkSingleton<SpawnerControl>
{
    [SerializeField]
    //The prefab that we use for instantiation
    private GameObject objectPrefab;

    [SerializeField]
    //To keep track how many instance we are going to make each time we spawn.  
    private int maxObjectInstanceCount = 3;

    /// <summary>
    /// To intialize a network pool 
    /// </summary>
    private void Awake()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            NetworkObjectPool.Instance.InitializePool();
        };
    }

    public void SpawnObjects()
    {
        //We  instantiate objects, we need to do it from the server, that why we check first
        if (!IsServer) return; 

        for (int i = 0; i < maxObjectInstanceCount; i++)
        {   
            //GameObject go = Instantiate(objectPrefab, 
            //    new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10)), Quaternion.identity);
           
            GameObject go = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab).gameObject;
            go.transform.position = new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10));//randomize the position and the height
        
            //this one is to sinchronize the objects on all clients
            //Notify other clients in order to instantionate it 
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}

