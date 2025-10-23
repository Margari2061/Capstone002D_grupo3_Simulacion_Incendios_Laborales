using AideTool;
using System.Collections;
using UnityEngine;

public class PlayController : MonoBehaviour
{
    [SerializeField] private PlayerHandler _player;
    [SerializeField] private Fire _firePrefab;
    [SerializeField] private GameoverController _gameoverCanvas;
    [SerializeField] private float _minStartTime = 8f;
    [SerializeField] private float _maxStartTime = 17f;
    [SerializeField] private int _levelCheck;

    private int _maxFires;

    [Foldout("Level 1"), SerializeField, InspectorName("Start Position")] private Vector3 _startPosition1;
    [SerializeField, InspectorName("Fire Positions")] private Vector3[] _firePositions1;

    [Foldout("Level 2"), SerializeField, InspectorName("Start Position")] private Vector3 _startPosition2;
    [SerializeField, InspectorName("Fire Positions")] private Vector3[] _firePositions2;
    [SerializeField] private GameObject[] _setToActive;

    private void OnEnable()
    {
        GameEvents.OnGameOver += GameOver;
    }

    private void Start()
    {
        _gameoverCanvas.gameObject.SetActive(false);
        int level = Persistence.Instance.TargetScenario;

        switch (level)
        {
            case 1:
                StartLevel1();
                break;
            case 2:
                StartLevel2();
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= GameOver;
    }

    private void StartLevel1()
    {
        _player.transform.position = _startPosition1;

        foreach(Vector3 position in _firePositions1)
        {
            float timer = Random.Range(_minStartTime, _maxStartTime);
            StartCoroutine(StartFireRoutine(position, timer, false));
        }
    }

    private void StartLevel2()
    {
        _player.transform.position = _startPosition2;

        int i = 0;
        foreach (Vector3 position in _firePositions2)
        {
            i++;
            float timer = Random.Range(_minStartTime, _maxStartTime);

            if(i == 1)
            {
                StartCoroutine(StartFireRoutine(position, timer, true));
                continue;
            }

            StartCoroutine(StartFireRoutine(position, timer, false));
        }
    }

    private IEnumerator StartFireRoutine(Vector3 position, float timer, bool alarm)
    {
        yield return new WaitForSeconds(timer);

        Instantiate(_firePrefab, position, Quaternion.identity);
        _maxFires++;

        if (alarm)
        {
            foreach(GameObject obj in _setToActive)
            {
                obj.SetActive(true);
            }
        }
    }

    private void GameOver(bool escape)
    {
        foreach (GameObject obj in _setToActive)
        {
            obj.SetActive(false);
        }

        RunResults result = Persistence.Instance.Data.FinishRun(escape, _maxFires);
        StartCoroutine(Persistence.Instance.FinishRun());

        _gameoverCanvas.gameObject.SetActive(true);
        _gameoverCanvas.SetTexts(result);
    }

    private void OnDrawGizmosSelected()
    {
        switch (_levelCheck)
        {
            case 1:
                DrawLevelGizmos(_startPosition1, _firePositions1);
                break;
            case 2:
                DrawLevelGizmos(_startPosition2, _firePositions2);
                break;
            default:
                break;
        }
    }

    private void DrawLevelGizmos(Vector3 player, Vector3[] fires)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(player, 0.5f);

        Gizmos.color = Color.orange;
        foreach (Vector3 position in fires)
        {
            Gizmos.DrawCube(position, 0.5f * Vector3.one);
        }
    }
}