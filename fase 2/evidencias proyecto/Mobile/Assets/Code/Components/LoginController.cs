using AideTool;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    private const string AuthPref = "authRut";

    [Header("Canvas")]
    [SerializeField] private Canvas _loginCanvas;
    [SerializeField] private Canvas _switchCanvas;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField _rutField;
    [SerializeField] private TMP_InputField _passwordField;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI _switchTitle;

    private string _prevRut;

    private void Start()
    {
        _prevRut = PlayerPrefs.GetString(AuthPref, "0");

        if(_prevRut == "0")
        {
            _switchCanvas.enabled = false;
            _loginCanvas.enabled = true;
            return;
        }

        _switchTitle.text = $"¿Seguir como {_prevRut}?";
        _loginCanvas.enabled = false;
        _switchCanvas.enabled = true;
    }

    public void LoginButton() => StartCoroutine(LoginRoutine());

    private IEnumerator LoginRoutine()
    {
        string rut = _rutField.text.ToLower();
        string password = _passwordField.text;

        ResponseResult response = null;
        yield return StartCoroutine(Persistence.Instance.LoginRoutine(rut, password, (r) => response = r));

        if(!response.CheckResponse())
        {
            yield break;
        }

        PlayerPrefs.SetString(AuthPref, rut);
        Persistence.Instance.UserRut = rut;
        SceneManager.LoadScene((int)Scenes.SceneSelector);
    }

    public void YesButton()
    {
        Persistence.Instance.UserRut = _prevRut;
        SceneManager.LoadScene((int)Scenes.SceneSelector);
    }

    public void NoButton()
    {
        _prevRut = "0";
        PlayerPrefs.SetString(AuthPref, "0");
        _switchCanvas.enabled = false;
        _loginCanvas.enabled = true;
    }
}