using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequest
{
    public delegate void RequestCallback<T>(T data);

    public async void MakeRequest<T>(string url, RequestCallback<T> callback)
    {
        T data = await SendRequest<T>(url);
        callback?.Invoke(data);
    }

    private async Task<T> SendRequest<T>(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var asyncOperation =  www.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield(); // You can adjust the delay as needed
            }

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
                return default;
            }
            else
            {
                string jsonText = www.downloadHandler.text;
                T parsedData = JsonUtility.FromJson<T>(jsonText);

                if (parsedData != null)
                {
                    return parsedData;
                }
                else
                {
                    Debug.LogError("Failed to parse JSON data.");
                    return default;
                }
            }
        }
    }
}