using AideTool;
using AideTool.Geometry;
using AideTool.Input.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    [Foldout("References"), SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [SerializeField] private SkinnedMeshRenderer _characterRenderer;
    [SerializeField] private Material _stillMaterial;
    [SerializeField] private Material _alertMaterial;
    [SerializeField] private GameObject _useButton;
    [EndFoldout, SerializeField] private GameObject _unequipButton;

    private CharacterAnimatorHandler _animatorHandler;

    [Foldout("Sensor"), SerializeField] private Vector3 _offset;
    [EndFoldout, SerializeField] private Vector3 _extents;

    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _rotationFactor;

    private InputAxis2D MoveInput { get; set; } = new();
    private InputButton UseInput { get; set; } = new();
    private InputButton UnequipButton { get; set; } = new();

    private IUsableObject _usable = null;
    private Extinguisher _equippedExtinguisher = null;

    public void OnMove(InputAction.CallbackContext context) => MoveInput.SetValues(context);
    public void OnUse(InputAction.CallbackContext context) => UseInput.SetValues(context);
    public void OnUnequip(InputAction.CallbackContext context) => UnequipButton.SetValues(context);

    private void Start()
    {
        _animatorHandler = new(_animator);
        _animatorHandler.Alert.Set(false);
        _animatorHandler.RunningLevel.Set(0);

        _useButton.SetActive(false);
        _unequipButton.SetActive(false);
    }

    private void Update()
    {
        HandleMovement();

        HandleButtonsVisibility();

        HandleUseButton();
        HandleUnequipButton();
    }

    private void FixedUpdate()
    {
        HandleSensor();
    }

    private void HandleMovement()
    {
        if (!MoveInput.IsActive)
        {
            _animatorHandler.RunningLevel.Set(0);
            return;
        }

        Quaternion cameraRotation = Quaternion.LookRotation(Camera.main.transform.forward);
        Vector3 vector = MoveInput.Axis.ToXZVector3();
        Vector3 rotatedVector = cameraRotation * vector;
        rotatedVector.y = 0f;

        float handleSpeed()
        {
            float x = Mathf.Abs(vector.x);
            float z = Mathf.Abs(vector.z);

            if (x > 0.5f || z > 0.5f)
            {
                _animatorHandler.RunningLevel.Set(2);
                return _runSpeed;
            }
            _animatorHandler.RunningLevel.Set(1);
            return _walkingSpeed;
        }

        float speed = handleSpeed();

        _controller.Move(speed * Time.deltaTime * rotatedVector);
        HandleRotation(rotatedVector);
    }

    private void HandleRotation(Vector3 vector)
    {
        if (vector.magnitude < 0.1f)
            return;

        Quaternion current = transform.rotation;
        Quaternion target = Quaternion.LookRotation(vector);

        transform.rotation = Quaternion.Slerp(current, target, _rotationFactor);
    }

    private void HandleUseButton()
    {
        if (_usable != null && UseInput.IsPressed)
        {
            _usable.Use();
            //_characterRenderer.material = _alertMaterial;
        }
    }

    private void HandleUnequipButton()
    {

    }

    private Box ObjectSensor()
    {
        Box box = new Box
        (
            transform.position + transform.rotation * _offset,
            _extents,
            transform.rotation
        );

        return box;
    }

    private void HandleSensor()
    {
        Box sensor = ObjectSensor();
        Collider[] colliders = Physics.OverlapBox(sensor.Origin, sensor.HalfExtents);

        foreach (Collider collider in colliders)
        {
            if(collider.transform.TryGetComponent(out IUsableObject obj))
            {
                _usable = obj;
                return;
            }
        }

        _usable = null;
    }

    private void HandleButtonsVisibility()
    {
        _useButton.SetActive(_usable != null);
        _unequipButton.SetActive(_equippedExtinguisher != null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        AideGizmo.DrawBox(ObjectSensor());
    }
}
