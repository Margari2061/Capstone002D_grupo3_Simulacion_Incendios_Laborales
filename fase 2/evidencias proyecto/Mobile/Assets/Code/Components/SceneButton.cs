using AideTool;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _buttonImage;
    [SerializeField] private TextMeshProUGUI _buttonNumber;
    [SerializeField] private Sprite _background;

    [Header("Variables")]
    [SerializeField] private int _targetScenario;

    public void GoScene() => StartCoroutine(GoSceneRoutine());

    private IEnumerator GoSceneRoutine()
    {
        Persistence.Instance.TargetScenario = _targetScenario;
        ResponseResult result = null;
        yield return StartCoroutine(Persistence.Instance.StartRun((r) => result = r));

        if (!result.CheckResponse())
            yield break;

        SceneManager.LoadScene((int)Scenes.PlayScene);
    }

    private void OnValidate()
    {
        if(_buttonNumber != null)
        {
            _buttonNumber.text = _targetScenario
                .ToString()
                .PadLeft(2, '0');
        }

        if (_background != null && _buttonImage != null)
        {
            _buttonImage.sprite = _background;
        }
    }
}