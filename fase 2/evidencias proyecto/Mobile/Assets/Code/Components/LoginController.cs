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
    private string _prevPass;

    private void Start()
    {
        string prefs = PlayerPrefs.GetString(AuthPref, "0&7");
        string[] teils = prefs.Split('&');
        _prevRut = teils[0];
        _prevPass = teils[1];

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

    public void LoginButton()
    {
        string rut = _rutField.text.ToLower();
        string password = _passwordField.text;
        StartCoroutine(LoginRoutine(rut, password));
    }

    public void YesButton() => StartCoroutine(LoginRoutine(_prevRut, _prevPass));

    public void NoButton()
    {
        _prevRut = "0";
        _prevPass = "7";
        PlayerPrefs.SetString(AuthPref, "0&7");
        _switchCanvas.enabled = false;
        _loginCanvas.enabled = true;
    }

    private IEnumerator LoginRoutine(string rut, string password)
    {
        ResponseResult response = null;
        yield return StartCoroutine(Persistence.Instance.LoginRoutine(rut, password, (r) => response = r));

        if(!response.CheckResponse())
        {
            _passwordField.text = "";
            yield break;
        }

        string[] teils = rut.Split('-');
        int rutNum = int.Parse(teils[0].Replace(".", ""));

        PlayerPrefs.SetString(AuthPref, $"{rut}&{password}");
        Persistence.Instance.UserRut = rutNum;
        SceneManager.LoadScene((int)Scenes.SceneSelector);
    }
}