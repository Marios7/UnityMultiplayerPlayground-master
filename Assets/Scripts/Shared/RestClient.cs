using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Weather
{
    public int id;
    public string main;
}
[Serializable]
public class WeatherInfo
{
    public int id;
    public string name;
    public List<Weather> weather;
}

public class RestClient : MonoBehaviour
{
    public static HttpClient client = new HttpClient();

    private const string API_KEY = "173de5ebbee2074cbf8be66d229287f5";
    private const string url = "http://api.openweathermap.org/data/2.5/weather?q=";
    public static string CityId = "170654";
    public static string DamascusCityString = "Damascus";
    //public static string WeatherURL = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}", DamascusCityString, API_KEY);

    //private static void InitializeClient(string cityName)
    //{
    //    //client = new HttpClient();
    //    client.BaseAddress = new Uri(localUrl);
    //    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    //    client.DefaultRequestHeaders.Accept.Clear();
    //    // Add an Accept header for JSON format.
    //    client.DefaultRequestHeaders.Accept.Add(
    //        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    //}
    //public static string sendRequest()
    //{
    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(localUrl);
    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    StreamReader reader = new StreamReader(response.GetResponseStream());
    //    string jsonResponse = reader.ReadToEnd();
    //    return jsonResponse;
    //}
    public static async Task<WeatherInfo> sendRequestAsync(string cityName)
    {
        string WeatherURL = string.Format("{0}{1}&APPID={2}", url,cityName, API_KEY);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(WeatherURL);
        HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        //Here I shoud Serialize 
        WeatherInfo info = JsonUtility.FromJson<WeatherInfo>(jsonResponse);
        return info;
    }

}
