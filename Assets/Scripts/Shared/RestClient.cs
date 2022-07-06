using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class RestClient : MonoBehaviour
{
    public static HttpClient client = new HttpClient();
    public static string url = "https://localhost:7027/api/Unity";
    public static string response = string.Empty;
    private static void InitializeClient()
    {
        //client = new HttpClient();
        client.BaseAddress = new Uri(url);
        System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        client.DefaultRequestHeaders.Accept.Clear();
        // Add an Accept header for JSON format.
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        
    }
    //private WeatherInfo GetWeather()
    //{
    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://api.openweathermap.org/data/2.5/weather?id={0}&APPID={1}", CityId, API_KEY));
    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    StreamReader reader = new StreamReader(response.GetResponseStream());
    //    string jsonResponse = reader.ReadToEnd();
    //    WeatherInfo info = JsonUtility.FromJson<WeatherInfo>(jsonResponse);
    //    return info;
    //}
    public static string sendRequest()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        return jsonResponse;
    }
}
