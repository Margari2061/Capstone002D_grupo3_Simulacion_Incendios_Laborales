using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private float _secondsToExtinguish = 3f;
    [SerializeField] private float _damageRadius;

    private float _fireLife;

    private void Start()
    {
        _fireLife = 1f;
    }

    private void FixedUpdate()
    {
        HandleDamage();
    }

    private void OnDestroy()
    {
        Persistence.Instance.Data.FuegosApagados++;
    }

    public void ReceiveFoam()
    {
        _fireLife -= (1f / _secondsToExtinguish) * Time.deltaTime;

        if(_fireLife <= 0f)
            Destroy(gameObject);
    }

    private void HandleDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _damageRadius);
        foreach(Collider collider in colliders)
        {
            if(collider.transform.TryGetComponent(out PlayerHandler player))
            {
                player.ReceiveDamage();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _damageRadius * 0.5f);
    }
}

