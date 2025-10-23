using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out PlayerHandler _))
            GameEvents.GameOver(true);
    }
}
