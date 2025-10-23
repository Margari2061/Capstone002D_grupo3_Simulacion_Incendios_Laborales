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
        { RunResults.CondicionesCumplidas, "Felicitaciones. Haz cumplido con todas las condiciones del escenario simulado." },
        { RunResults.EscapeInmediato, "Haz logrado ponerte a salvo según los protocolos establecidos para un empleado que no es monitor de piso." },
        { RunResults.EscapeSeguro, "Bien hecho. Has priorizado tu seguridad y reconocido cuando la situación no está bajo tu control." },
        { RunResults.EscapeTardio, "No te hagas el héroe. En una situación real podrías haber sido gravemente herido. Y nadie quiere eso." },
        { RunResults.Muerte, "No solo te has herido a tí mismo, sino que has puesto en peligro a tus compañeros de trabajo. Recuerda mantener la calma y seguir los protocolos establecidos." },
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