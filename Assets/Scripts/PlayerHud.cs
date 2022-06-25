using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    //If you also want Unity to serialize your private fields you can add the SerializeField attribute to those fields.
    [SerializeField]
    private NetworkVariable<NetworkString> playerNetworkName = new NetworkVariable<NetworkString>();

    private bool overlaySet = false;

    //Very important method
    //To catch at what point a player is getting spawn
    //Here we set the overlay of the player
    public override void OnNetworkSpawn()
    {
        if(IsServer)//Just the server is able to do this 
        {
            playerNetworkName.Value = $"Player {OwnerClientId}";
        }
    }

    //this method is to set the overlay of the player (The phrase that appears upper the player, and in the log which has it's id..)
    public void SetOverlay()
    {
        //we don't need to check if it's server or not because the vairable is readable from everywhere but just the server can write it
        var localPlayerOverlay = gameObject .GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = $"{playerNetworkName.Value}";
    }

    public void Update()
    {
        if(!overlaySet && !string.IsNullOrEmpty(playerNetworkName.Value))
        {
            SetOverlay();
            overlaySet = true;
        }
    }
}
 