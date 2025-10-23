using UnityEngine;

public class Alarm : MonoBehaviour, IUsableObject
{
    [SerializeField] private GameObject[] _setToActive;

    public void Use(PlayerHandler player)
    {
        bool active = !_setToActive[0].activeInHierarchy;
        Persistence.Instance.Data.UsoAlarma = active;

        foreach (GameObject obj in _setToActive)
        {
            obj.SetActive(active);
        }
    }
}