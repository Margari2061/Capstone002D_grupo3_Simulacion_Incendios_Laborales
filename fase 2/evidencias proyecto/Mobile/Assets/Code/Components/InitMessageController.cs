using AideTool.Input.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitMessageController : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private Image _tapIcon;

    private bool _tapEnabled = false;
    private readonly InputButton _tapInput = new();

    private IEnumerator Start()
    {
        _tapIcon.enabled = false;

        yield return new WaitForSeconds(_timer);

        _tapIcon.enabled = true;
        _tapEnabled = true;
    }

    private void Update()
    {
        if (_tapEnabled && _tapInput.IsPressed)
            SceneManager.LoadScene((int)Scenes.Login);
    }

    public void OnTap(InputAction.CallbackContext context) => _tapInput.SetValues(context);
}
