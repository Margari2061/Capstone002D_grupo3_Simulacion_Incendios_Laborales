using AideTool;
using System;
using System.Collections;

public class Persistence
{
    public string UserRut { get; set; }

    public int TargetScenario { get; set; }

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
        callback(ResponseResult.Ok());
        yield break;
    }
}
