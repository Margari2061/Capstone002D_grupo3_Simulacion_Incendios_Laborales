using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform m_follow;
    [SerializeField] private Vector3 m_positionOffset;
    [SerializeField] private Vector3 m_rotationOffset;

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        transform.position = m_follow.position + m_positionOffset;
        transform.LookAt(m_follow.position + m_rotationOffset);
    }

    private void OnValidate()
    {
        if (m_follow != null)
            HandleMovement();
    }
}
