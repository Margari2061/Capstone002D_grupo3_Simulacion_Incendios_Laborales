using AideTool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameoverController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _gameoverButton;
    [SerializeField] private Image _indicator;

    private readonly Dictionary<RunResults, string> _textOptions = new()
    {
        { RunResults.CondicionesCumplidas, "Felicitaciones. Haz cumplido con todas las condiciones del escenario simulado." },
        { RunResults.EscapeInmediato, "Haz logrado ponerte a salvo según los protocolos establecidos para un empleado que no es monitor de piso." },
        { RunResults.EscapeSeguro, "Bien hecho. Has priorizado tu seguridad y reconocido cuando la situación no está bajo tu control." },
        { RunResults.EscapeTardio, "No te hagas el héroe. En una situación real podrías haber sido gravemente herido. Y nadie quiere eso." },
        { RunResults.Muerte, "No solo te has herido a tí mismo, sino que has puesto en peligro a tus compañeros de trabajo. Recuerda mantener la calma y seguir los protocolos establecidos." },
    };

    private void Start()
    {
        _playAgainButton.interactable = false;
        _gameoverButton.interactable = false;
    }

    private void Update()
    {
        if(Persistence.Instance.FinalTransferReady)
        {
            _playAgainButton.interactable = true;
            _gameoverButton.interactable = true;
            _indicator.enabled = false;
            return;
        }

        _indicator.transform.Rotate(0f, 0f, Time.deltaTime);
    }

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