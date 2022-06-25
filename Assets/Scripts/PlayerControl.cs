using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed = 2.8f;

    [SerializeField]
    private float runSpeedOffset = 2.0f;

    [SerializeField]
    private float rotationSpeed = 3.5f;


    [SerializeField]
    //Deafualt position of the player on the plane
    private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);


    /*  * we can add permessions in the constructors
        * NetworkVariableReadPermission.Everyone means that every one can read it
        * By deafault, everyone can read it and just the server can write
    */
    [SerializeField]
    private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();


    [SerializeField]
    //keep track of the player left and right rotation
    private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();

    [SerializeField]
    //To know which animation to play
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    private CharacterController characterController;

    // client caches positions
    private Vector3 oldInputPosition = Vector3.zero;
    private Vector3 oldInputRotation = Vector3.zero;
    private PlayerState oldPlayerState = PlayerState.Idle;

    private Animator animator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (IsClient && IsOwner)
        {
            //position the player randomly
            transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                   Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y));
        }
    }

    void Update()
    {
        if (IsClient && IsOwner) //If we do not add the (IsOwner) check, we could controll other players and it would make no sense
        {
            ClientInput();
        }

        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void ClientMoveAndRotate()
    {
        //Moving forward and backward
        if (networkPositionDirection.Value != Vector3.zero)
        {
            characterController.SimpleMove(networkPositionDirection.Value);
        }
        //Rotation 
        if (networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRotationDirection.Value, Space.World);
        }
    }

    //change the animation 
    private void ClientVisuals()
    {
        if (oldPlayerState != networkPlayerState.Value)
        {
            oldPlayerState = networkPlayerState.Value;
            animator.SetTrigger($"{networkPlayerState.Value}");
        }
    }

    private void ClientInput()
    {
        // left & right rotation
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        // forward & backward direction 
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        // change animation states
        if (forwardInput == 0) //Idle 
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        else if (!ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1) //walking
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)//Runing 
        {
            inputPosition = direction * runSpeedOffset;
            UpdatePlayerStateServerRpc(PlayerState.Run);
        }
        else if (forwardInput < 0)
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);//In order to change the animation 

        // let server know about position and rotation client changes
        if (oldInputPosition != inputPosition ||
            oldInputRotation != inputRotation)
        {
            oldInputRotation = inputRotation;
            oldInputPosition = inputPosition;
            //whenevre we make a call to Rpc Server, The method should have "ServerRpc" postfix at the end of it's name
            //otherwise VS will complain 
            UpdateClientPositionAndRotationServerRpc(inputPosition * walkSpeed, inputRotation * rotationSpeed);
        }
    }

    private static bool ActiveRunningActionKey()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition, Vector3 newRotation)
    {
        networkPositionDirection.Value = newPosition;
        networkRotationDirection.Value = newRotation;
    }

    //The [ServerRpc] attribuite tell the networking system to excute this method on the server, we call it from the client but we excute it on the server
    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
    }
}
