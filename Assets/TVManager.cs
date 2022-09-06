using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using B83.Win32;
using UnityEngine.Video;
using System;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using System.IO;


public class TVManager : MonoBehaviour
{
    public TextMeshProUGUI ApiResponseTextMesh;
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

    private string url = "";
    private string defaultVideoUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerJoyrides.mp4";
    private string cityName = "";
    //The colors to use when focus
    private Color mouseOverColor = Color.cyan;
    private Color originalButtonColor;
    VideoPlayer videoPlayer = null;
    //the ray is used to check where the mouse points
    Ray ray;
    RaycastHit hit;
    Renderer TVRenderer;

    private void Start()
    {
        TVRenderer = GetComponent<Renderer>();
        videoPlayer = GetComponent<VideoPlayer>();

        callRestApiButton.onClick.AddListener(CallRestApiEvent);
        StopButton.onClick.AddListener(StopVideo);
        PauseButton.onClick.AddListener(PauseVideo);
        PlayButton.onClick.AddListener(PlayVideo);
        //We can't initiat the color directly with the definition
        originalButtonColor = TVRenderer.material.color;
    }

    private async void CallRestApiEvent()
    {
        Logger.Instance.LogInfo("Call RestFulService..");
        cityName = CityFieldTextBox.text;
        if (String.IsNullOrEmpty(cityName))
            cityName = RestClient.DamascusCityString;
        if (videoPlayer != null && videoPlayer.isPlaying)
            StopVideo();
        try
        {
            WeatherInfo s = await RestClient.sendRequestAsync(cityName);
            var MessageToShow = $"City: {s.name}\n" +
                                $"City Id: {s.id}\n" +
                                $"Weather: {s.weather.FirstOrDefault().main}d";
            ApiResponseTextMesh.text = MessageToShow;
        }
        catch (Exception ex)
        {
            Logger.Instance.LogError($"Exception: {ex.Message}");
        }
    }


    void OnEnable()
    {
        //For drag and drop
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {   //Drag and drop
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        try
        {
            PlayViedoWhenTheObjectIsTV(aFiles[0]);
            string str = "Dropped " + aFiles.Count + " files at: " + aPos + "\n\t" +
                aFiles.Aggregate((a, b) => a + "\n\t" + b);
            Logger.Instance.LogInfo(str);
        }
        catch (Exception ex)
        {
            Logger.Instance.LogError($"Error! {ex.ToString()}");
        }
    }

    private bool isTV()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider.name == "SteeviTheTV")
        {
            TVRenderer.material.color = mouseOverColor;
            return true;
        }
        TVRenderer.material.color = originalButtonColor;
        return false;
    }
    private void PlayViedoWhenTheObjectIsTV(string url)
    {
        if (isTV())
        {
            Logger.Instance.LogInfo($"Video Url is updated: {url}");
            videoPlayer.url = url;
        }
    }
    public void StopVideo()
    {
        Logger.Instance.LogInfo("Stopping Video..");

        if (videoPlayer != null)
            videoPlayer.Stop();
    }
    public void PlayVideo()
    {
        Logger.Instance.LogInfo("Playing Video...");
        ApiResponseTextMesh.text = "";
        ApiResponseTextMesh.text = "";
        if (videoPlayer != null && (!videoPlayer.isPlaying || videoPlayer.isPaused))
        {
            url = VideoUrlTextBox.text;
            if (String.IsNullOrEmpty(url))
                url = defaultVideoUrl;
            {
                try
                {
                    /*_ = Path.GetFullPath(url);*///in order to check if the path is valid, if it's not, it will throw an Exception
                    videoPlayer.url = url;
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
    public void PauseVideo()
    {
        Logger.Instance.LogInfo("Pause Video Button clicked");
        if (videoPlayer != null && (videoPlayer.isPlaying))
            videoPlayer.Pause();

    }
}

