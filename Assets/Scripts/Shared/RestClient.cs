using DilmerGames.Core.Singletons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(NetworkBehaviour))]
public class RestClient : NetworkSingleton<RestClient>
{
    //Private Attributes
    private const string API_KEY = "173de5ebbee2074cbf8be66d229287f5";
    private const string url = "http://api.openweathermap.org/data/2.5/weather?q=";

    //Private Attributes
    public static string DamascusCityId = "170654";
    public static string DamascusCityString = "Damascus";
    public static NetworkVariable<NetworkString> CityName = new NetworkVariable<NetworkString>();

    //Network variables can be changed only from the server
    //This variable is the one responsabile of synchronizing the Api Response between the Server and the Clients (Note that the class inherits NetworkBehaviour and not MonoBehaviour).
    public NetworkVariable<NetworkString> networkApiMessage = new NetworkVariable<NetworkString>();

    public string Message
    {
        get
        {
            return networkApiMessage.Value;
        }
    }

    public void Start()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                networkApiMessage.Value = "Network Variable is Initiated";
            };
        }
    }
    public string sendRequest(string cityName)
    {
        //Initialize each time before the call
        ClearMessage();
        string WeatherURL = string.Format("{0}{1}&APPID={2}", url, cityName, API_KEY);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(WeatherURL);
        HttpWebResponse response = (HttpWebResponse)(request.GetResponse());
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        WeatherInfo info = JsonUtility.FromJson<WeatherInfo>(jsonResponse);
        networkApiMessage.Value = "The weather in " + info.name + " is " + info.weather.FirstOrDefault().main;
        Logger.Instance.LogInfo($"NetworkVariable.Value: {Message}");
        //Logger.Instance.LogInfo($"{info.weather.FirstOrDefault().main}");
        return Message;
    }
    public async Task<WeatherInfo> sendRequestAsync(string cityName)
    {
        //Initialize each time before the call
        ClearMessage();
        string WeatherURL = string.Format("{0}{1}&APPID={2}", url, cityName, API_KEY);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(WeatherURL);
        HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        WeatherInfo info = JsonUtility.FromJson<WeatherInfo>(jsonResponse);
        //Formatting the string with $ or with string.Format results in exception 
        networkApiMessage.Value = "The weather in \n" + info.name + " is " + info.weather.FirstOrDefault().main;
        Logger.Instance.LogInfo($"NetworkVariable.Value: {networkApiMessage.Value}");
        //Logger.Instance.LogInfo($"Weather: {info.weather.FirstOrDefault().main}");
        return info;
    }

    public void ClearMessage()
    {
        networkApiMessage.Value = "";
    }
}
