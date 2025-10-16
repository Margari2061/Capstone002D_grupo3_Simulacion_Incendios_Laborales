using UnityEngine;

public class Alarm : MonoBehaviour, IUsableObject
{
    [SerializeField] private GameObject _alarm;

    public void Use()
    {
        bool active = !_alarm.activeInHierarchy;

        _alarm.SetActive(active);
    }
}