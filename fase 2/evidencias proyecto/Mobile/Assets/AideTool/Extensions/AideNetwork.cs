using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

namespace AideTool
{
    public static class AideNetwork
    {
        public static UnityWebRequest GetJson<T>(string url, T data)
        {
            string jsonText = JsonConvert.SerializeObject(data);
            byte[] jsonBin = Encoding.UTF8.GetBytes(jsonText);

            DownloadHandlerBuffer downloadHandler = new();

            UploadHandlerRaw uploadHandler = new(jsonBin);
            uploadHandler.contentType = "application/json";

            UnityWebRequest outputRequest = new(url, "GET", downloadHandler, uploadHandler);
            return outputRequest;
        }

        public static UnityWebRequest PostJson<T>(string url, T data)
        {
            string jsonText = JsonConvert.SerializeObject(data);
            byte[] jsonBin = Encoding.UTF8.GetBytes(jsonText);

            DownloadHandlerBuffer downloadHandler = new();

            UploadHandlerRaw uploadHandler = new(jsonBin);
            uploadHandler.contentType = "application/json";

            UnityWebRequest outputRequest = new(url, "POST", downloadHandler, uploadHandler);
            return outputRequest;
        }

        public static UnityWebRequest PutJson<T>(string url, T data)
        {
            string jsonText = JsonConvert.SerializeObject(data);
            byte[] jsonBin = Encoding.UTF8.GetBytes(jsonText);

            DownloadHandlerBuffer downloadHandler = new();

            UploadHandlerRaw uploadHandler = new(jsonBin);
            uploadHandler.contentType = "application/json";

            UnityWebRequest outputRequest = new(url, "PUT", downloadHandler, uploadHandler);
            return outputRequest;
        }

        public static UnityWebRequest DeleteJson<T>(string url, T data)
        {
            string jsonText = JsonConvert.SerializeObject(data);
            byte[] jsonBin = Encoding.UTF8.GetBytes(jsonText);

            DownloadHandlerBuffer downloadHandler = new();

            UploadHandlerRaw uploadHandler = new(jsonBin);
            uploadHandler.contentType = "application/json";

            UnityWebRequest outputRequest = new(url, "DELETE", downloadHandler, uploadHandler);
            return outputRequest;
        }

        public static bool CheckResponse(this ResponseResult result)
        {
            if(result == null)
                return false;

            if (result.Status == RequestResultStatus.Failed)
                return false;

            return true;
        }

        public static bool CheckRequest(this UnityWebRequest request, out string message)
        {
            message = "";
            if (request.result != UnityWebRequest.Result.Success)
            {
                message = $"Text: {request.downloadHandler.text}\nError: {request.downloadHandler.error}";
                return false;
            }
            return true;
        }

        public static IEnumerator WhileProgress(this UnityWebRequest request, Action<ResponseResult> callback)
        {
            while (request.result == UnityWebRequest.Result.InProgress)
            {
                float progress = request.uploadProgress + request.downloadProgress;
                progress *= 0.5f;
                callback(ResponseResult.Progress(progress));
                yield return null;
            }
        }
    }
}
