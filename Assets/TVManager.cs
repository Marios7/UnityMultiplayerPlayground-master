using UnityEngine;
using UnityEngine.Video;

using UnityEngine.Networking;
using System;

using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using Unity.Netcode;


public class TVManager : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ApiResponseTextMesh;

    //public TextMeshProUGUI ApiResponseTextMesh;
    [SerializeField]
    private TMP_InputField VideoUrlTextBox;
    [SerializeField]
    private TMP_InputField CityFieldTextBox;

    //The TV buttons
    [SerializeField]
    private Button callRestApiButton;
    [SerializeField]
    private Button StopButton;
    [SerializeField]
    private Button PlayButton;
    [SerializeField]
    private Button PauseButton;

    //Default Variables
    private string defaultVideoUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerJoyrides.mp4";
    //The colors to use when focus
    private Color mouseOverColor = Color.cyan;
    private Color originalButtonColor;
    VideoPlayer videoPlayer = null;

    //the ray is used to check where the mouse points
    //Notice that for TV We did not associate the gameObjectChangeColor, that is because, in the drag and drop feature, we can drop just on the TV Obeject so we need to know if the mouse is pointing on the TV and that's what isTV() does.
    Ray ray;
    RaycastHit hit;
    Renderer TVRenderer;

    public void Update()
    {
        //if (IsClient)//We need to update it just on the clients beacuse in the server it's being changed in the first place so we don't have to update it there
        ApiResponseTextMesh.text = RestClient.Instance.networkApiMessage.Value;
    }
    private void Start()
    {
        TVRenderer = GetComponent<Renderer>();
        videoPlayer = GetComponent<VideoPlayer>();

        //Attach the Events to the buttons
        callRestApiButton.onClick.AddListener(CallRestApiEvent);
        StopButton.onClick.AddListener(StopVideoEvent);
        PauseButton.onClick.AddListener(PauseVideoEvent);
        PlayButton.onClick.AddListener(PlayVideoEvent);

        //We can't initiat the color directly with the definition
        originalButtonColor = TVRenderer.material.color;
    }
    //Network variables can be changed only from the server
    //In case the client press the button to call the API then we need to Invoked as if the server did call, in other words; tell the server to invoke the event and change the value of the network variable.
    //Once the value of the NetworkVariable is changed by the server, it will be syncronized across the clients as if the client call the API.
    #region Call Rest Api
    public void CallRestApiEvent()
    {
        if (IsServer)
            CallRestApi(CityFieldTextBox.text);
        else//if it's the client, then ask the server to call the api (then the server will share the message to all the client with the network variables)
            CallRestApIServerRpc(CityFieldTextBox.text);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CallRestApIServerRpc(string cityName)
    {
        CallRestApi(cityName);
    }

    private async void CallRestApi(string cityName)
    {
        //if (IsServer)
        //{
        Logger.Instance.LogInfo("Call RestFulService..");
        if (String.IsNullOrEmpty(cityName))
            cityName = RestClient.DamascusCityString;
        
        if (videoPlayer != null && videoPlayer.isPlaying)
            StopVideoEvent();
        try
        {
            //Calling the API will set the NetworkVariable "APIMessage" which will update the api Message showed on the Tv
            //The Response of the API is shared using NetworkVarible.
            //The city name is sent from the client to the server when the client change the city name, and ask the server to call the api with for this city.
            //When the server change the city name, it shares the whole response.
            _ = await RestClient.Instance.sendRequestAsync(cityName);
        }
        catch (Exception ex)
        {
            Logger.Instance.LogError($"Exception: {ex.Message}");
        }
        //}
    }
    #endregion
    //void OnEnable()
    //{
    //    //For drag and drop
    //    // must be installed on the main thread to get the right thread id.
    //    UnityDragAndDropHook.InstallHook();
    //    UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    //}
    //void OnDisable()
    //{   //Drag and drop
    //    UnityDragAndDropHook.UninstallHook();
    //}

    //void OnFiles(List<string> aFiles, POINT aPos)
    //{
    //    // do something with the dropped file names. aPos will contain the 
    //    // mouse position within the window where the files has been dropped.
    //    try
    //    {
    //        PlayViedoWhenTheObjectIsTV(aFiles[0]);
    //        string str = "Dropped " + aFiles.Count + " files at: " + aPos + "\n\t" +
    //            aFiles.Aggregate((a, b) => a + "\n\t" + b);
    //        Logger.Instance.LogInfo(str);
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.Instance.LogError($"Error! {ex.ToString()}");
    //    }
    //}

    //private bool isTV()
    //{
    //    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out hit) && hit.collider.name == "SteeviTheTV")
    //    {
    //        TVRenderer.material.color = mouseOverColor;
    //        return true;
    //    }
    //    TVRenderer.material.color = originalButtonColor;
    //    return false;
    //}

    //private void PlayViedoWhenTheObjectIsTV(string url)
    //{
    //    if (isTV())
    //    {
    //        Logger.Instance.LogInfo($"Video Url is updated: {url}");
    //        videoPlayer.url = url;
    //    }
    //}


    /// <summary>
    /// If the client is caling this method then we want all the clients, server included to run the video
    /// BUT the only Device that can talk to all the clients is the SERVER, so we use ServerRpc to call the server and from there we use ClientRpc to call all the clients.
    /// This methodology will be used in Play, Pause and Stop functions.
    /// </summary>
    #region Play Video
    public void PlayVideoEvent()
    {
        if (IsServer)
        {
            PlayVideoClientRpc(VideoUrlTextBox.text);
        }
        else
        {
            PlayVideoServerRpc(VideoUrlTextBox.text);
        }
    }

    [ServerRpc(RequireOwnership = false)]//The requireOwner=false is neccessary otherwse it will require owenship and it will not be able to call it
    private void PlayVideoServerRpc(string VideoUrl)
    {
        PlayVideoClientRpc(VideoUrl);
    }
    [ClientRpc]
    private void PlayVideoClientRpc(string VideoUrl)
    {
        Logger.Instance.LogInfo("Playing Video...");
        //Clean the board in case there is a Message;
        if (IsServer)
            RestClient.Instance.ClearMessage();
        if (videoPlayer != null && (!videoPlayer.isPlaying || videoPlayer.isPaused))
        {
            if (String.IsNullOrEmpty(VideoUrl))
                VideoUrl = defaultVideoUrl;
            {
                try
                {
                    /*_ = Path.GetFullPath(url);*///in order to check if the path is valid, if it's not, it will throw an Exception
                    videoPlayer.url = VideoUrl;
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogError($"Error! {ex.Message}");
                    return;
                }
            }
        }
        videoPlayer.Play();
    }
    #endregion

    #region Pause Video
    private void PauseVideoEvent()
    {
        if (IsServer)//if it's the server then just play it with ClientRpc
        {
            PauseVideoClientRpc();
        }
        else//if it's client then trigger the server to play the video with ServerRpc
        {
            PauseVideoServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void PauseVideoServerRpc()
    {
        PauseVideoClientRpc();
    }
    [ClientRpc]
    public void PauseVideoClientRpc()
    {
        Logger.Instance.LogInfo("Pause Video Button clicked");
        if (videoPlayer != null && (videoPlayer.isPlaying))
            videoPlayer.Pause();

    }
    #endregion

    #region Stop Video
    private void StopVideoEvent()
    {
        if (IsServer)
            StopVideoClientRpc();
        else
            StopVideoServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopVideoServerRpc()
    {
        StopVideoClientRpc();
    }
    [ClientRpc]
    private void StopVideoClientRpc()
    {
        Logger.Instance.LogInfo("Stopping Video");
        if (videoPlayer != null)
            videoPlayer.Stop();
    }
    #endregion
}

