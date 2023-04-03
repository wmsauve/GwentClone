using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BackendFunctionality
{
    [System.Serializable]
    public struct UserData
    {
        public string username;
        public string password;
    }
    [System.Serializable]
    public struct ResponseFromServer
    {
        public string message;
        public string information;
        public bool isSuccess;
    }

    public class APIManager: Singleton<APIManager>
    {
        [Header("URL Related")]
        [SerializeField] private string m_apiURL;
        [SerializeField] private string m_loginEndpoint;
        [SerializeField] private string m_signupEndpoint;

        public string API_URL { get { return m_apiURL; } }
        public string API_ENDPOINT_LOGIN { get { return m_loginEndpoint; } }
        public string API_ENDPOINT_SIGNUP { get { return m_signupEndpoint; } }

        public IEnumerator GetRequest(string url, EnumAPIType type)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // Print response data
                    Debug.Log(www.downloadHandler.text);
                }
            }
        }

        public IEnumerator PostRequest<T>(string url, T data, EnumAPIType type)
        {
            var request = new UnityWebRequest(url, "POST");
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            //Debug.Log("Status Code: " + request.responseCode);
            //Debug.Log("Message: " + request.downloadHandler.text);

            ResponseFromServer _fromServer = new ResponseFromServer();
            if (request.responseCode == 200)
            {
                _fromServer = JsonUtility.FromJson<ResponseFromServer>(request.downloadHandler.text);
                Debug.Log(_fromServer.message);
                Debug.Log(_fromServer.isSuccess);
                Debug.Log(_fromServer.information);
            }
            GlobalBackendActions.OnFinishedAPICall?.Invoke(type, _fromServer, (int)request.responseCode);
            request.Dispose();
            yield return null;
        }
    }

    
}

