using AideTool;
using AideTool.Input.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    [Foldout("References"), SerializeField, EndFoldout] private CharacterController _controller;

    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _runSpeed;

    private InputAxis2D MoveInput { get; set; } = new();
    private InputButton UseInput { get; set; } = new();
    private InputButton UnequipButton { get; set; } = new();

    public void OnMove(InputAction.CallbackContext context) => MoveInput.SetValues(context);
    public void OnUse(InputAction.CallbackContext context) => UseInput.SetValues(context);
    public void OnUnequip(InputAction.CallbackContext context) => UnequipButton.SetValues(context);

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Quaternion cameraRotation = Quaternion.LookRotation(Camera.main.transform.forward);
        Vector3 vector = MoveInput.Axis.ToXZVector3();
        Vector3 rotatedVector = cameraRotation * vector;
        rotatedVector.y = 0f;

        float handleSpeed()
        {
            if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f)
                return _runSpeed;
            return _walkingSpeed;
        }

        float speed = handleSpeed();

        _controller.Move(speed * Time.deltaTime * rotatedVector);
    }

    private void HandleRotation()
    {

    }
}
