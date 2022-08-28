using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using B83.Win32;
using UnityEngine.Video;

using System;
using System.Windows;
using TMPro;

public class TVManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;

    [SerializeField]
    public string VideoUrl = "";
    private Color mouseOverColor = Color.cyan;
    private Color originalColor;
    VideoPlayer videoPlayer = null;
    private Renderer renderer;

    Ray ray;
    RaycastHit hit;
    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }
    void Update()
    {
        ClientInput();
    }

    private void ClientInput()
    {
        try
        {
            string bufferString = GUIUtility.systemCopyBuffer;
            if (!String.IsNullOrEmpty(bufferString) && Uri.IsWellFormedUriString(bufferString, UriKind.Absolute))
            {
                string url = bufferString;
                if (isTV())//Check if we are pointing at the tv
                {
                    textMesh.text = $"Press 'V' to play the video from the link:\n {url}";
                }
                else
                {
                    textMesh.text = "";
                }
                if (Input.GetKey(KeyCode.V))
                {
                    if (isTV())
                    {
                        Logger.Instance.LogInfo($"Trying to play clip from: {url}");
                        if (videoPlayer == null)
                        {
                            videoPlayer = GetComponent<VideoPlayer>();
                            videoPlayer.url = url;
                            videoPlayer.Play();
                        }
                    }
                    //PlayViedoWhenTheObjectIsTV(url);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Instance.LogError($"Error! {ex.Message}");
        }
    }
    void OnMouseEnter()
    {
        renderer.material.color = mouseOverColor;
    }

    void OnMouseExit()
    {
        renderer.material.color = originalColor;
    }

    void OnEnable()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
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
            return true;
        return false;
    }
    private void PlayViedoWhenTheObjectIsTV(string url)
    {
        if (isTV())
        {
            Logger.Instance.LogInfo($"Trying to play clip from: {url}");
            videoPlayer.url = url;
            videoPlayer.Play();
        }

    }
}

