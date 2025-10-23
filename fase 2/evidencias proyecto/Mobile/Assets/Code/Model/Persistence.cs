using AideTool;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine.Networking;

public class Persistence
{
    private const string Host = "https://localhost:7212";
    private string _tokenKey = "";
    private DateTime _tokenExpiration;

    public int UserRut { get; set; }
    public RunDataTransfer Data { get; private set; }
    public bool FinalTransferReady { get; private set; } = false;

    public int TargetScenario { get; set; }

    private Persistence()
    {
        Data = new();
    }

    private static Persistence _instance;
    public static Persistence Instance
    {
        get
        {
            if(_instance == null)
                _instance = new();
            return _instance;
        }
    }

    public IEnumerator LoginRoutine(string username, string password, Action<ResponseResult> callback)
    {
        using(UnityWebRequest request = AideNetwork.PostJson($"{Host}/api/auth", new {Rut = username, Password=password}))
        {
            yield return request.SendWebRequest();

            while (!request.isDone)
                yield return null;

            string response = request.downloadHandler.text;
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(response);

            if (token == null || !token.Status)
            {
                callback(ResponseResult.Fail(""));
                yield break;
            }

            _tokenKey = token.Key;
            _tokenExpiration = token.Expires;
        }

        callback(ResponseResult.Ok());
    }

    public IEnumerator StartRun(Action<ResponseResult> callback)
    {
        RunCreateRequest data = new()
        {
            RutUsuario = UserRut,
            Nivel = TargetScenario
        };

        using(UnityWebRequest request = AideNetwork.PostJson($"{Host}/api/runStart", data))
        {
            request.SetRequestHeader("Authorization", _tokenKey);
            yield return request.SendWebRequest();

            while (!request.isDone)
                yield return null;

            string response = request.downloadHandler.text;
            RunCreateResponse responseDeserial = JsonConvert.DeserializeObject<RunCreateResponse>(response);

            if(responseDeserial == null)
            {
                callback(ResponseResult.Fail(""));
                yield break;
            }

            Data.Id = responseDeserial.PartidaId;
            callback(ResponseResult.Ok());
        }
    }

    public IEnumerator FinishRun()
    {
        using (UnityWebRequest request = AideNetwork.PostJson($"{Host}/api/test", ""))
        {
            request.SetRequestHeader("Authorization", _tokenKey);
            yield return request.SendWebRequest();

            while (!request.isDone)
                yield return null;

            string response = request.downloadHandler.text;
            if (response != "OK")
            {
                yield break;
            }

            FinalTransferReady = true;
        }
    }
}
