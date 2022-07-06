using DilmerGames.Core.Singletons;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private Button startServerButton;
    
    [SerializeField]
    private Button CallRestButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField]
    //The server controls and spawns these object that appear when we click excutePhysicsButtons
    private Button executePhysicsButton;

    private bool hasServerStarted;

    private void Awake()
    {
        //Show the cursor when we are in a multiple unity sessions so that we can select each one of them 
        Cursor.visible = true;
    }

    void Update()
    {
        //PlayersInGame is a network variable 
        playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
        //Player manager is a kind of the loby, it keeps count of the players and stuff, like who is connecting and what's their id
    }

    void Start()
    {
        CallRestButton?.onClick.AddListener(() =>
        {
            Logger.Instance.LogInfo("Call RestFulService..");
            try
            {
                var s = RestClient.sendRequest();
                Logger.Instance.LogInfo($"RestFulService says: {s}");

            }
            catch (Exception ex)
            {
                Logger.Instance.LogInfo($"Exception: {ex.Message}");
            }
        });

        //Network Manager is from .NetCode
        // START SERVER
        startServerButton?.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
                Logger.Instance.LogInfo("Server started...");
            else
                Logger.Instance.LogInfo("Unable to start server...");
        });

        // START HOST
        startHostButton?.onClick.AddListener(async () =>
        {
            // this allows the UnityMultiplayer and UnityMultiplayerRelay scene to work with and without
            // relay features - if the Unity transport is found and is relay protocol then we redirect all the 
            // traffic through the relay, else it just uses a LAN type (UNET) communication.
            if (RelayManager.Instance.IsRelayEnabled) 
                await RelayManager.Instance.SetupRelay();

            if (NetworkManager.Singleton.StartHost())
                Logger.Instance.LogInfo("Host started...");
            else
                Logger.Instance.LogInfo("Unable to start host...");
        });

        // START CLIENT
        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);

            if(NetworkManager.Singleton.StartClient())
                Logger.Instance.LogInfo("Client started...");
            else
                Logger.Instance.LogInfo("Unable to start client...");
        });

        

        // STATUS TYPE CALLBACKS
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Logger.Instance.LogInfo($"{id} just connected...");
        };

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };

        executePhysicsButton.onClick.AddListener(() => 
        {
            if (!hasServerStarted)
            {
                Logger.Instance.LogWarning("Server has not started...");
                return;
            }
            SpawnerControl.Instance.SpawnObjects();
        });
    }
}
