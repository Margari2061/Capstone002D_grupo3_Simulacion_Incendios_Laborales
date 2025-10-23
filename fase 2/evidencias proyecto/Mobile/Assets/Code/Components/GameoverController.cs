using AideTool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private readonly Dictionary<RunResults, string> _textOptions = new()
    {
        { RunResults.CondicionesCumplidas, "" },
        { RunResults.EscapeInmediato, "" },
        { RunResults.EscapeSeguro, "" },
        { RunResults.EscapeTardio, "" },
        { RunResults.Muerte, "" },
    };

    public void SetTexts(RunResults result)
    {
        _text.text = _textOptions[result];
    }

    public void PlayAgain()
    {
        if(Persistence.Instance.FinalTransferReady)
            SceneManager.LoadScene((int)Scenes.SceneSelector);
    }

    public void Quit()
    {
        if (Persistence.Instance.FinalTransferReady)
            Aide.ExitApplication();
    }
}