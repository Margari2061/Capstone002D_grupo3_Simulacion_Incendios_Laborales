using AideTool;
using AideTool.Geometry;
using AideTool.Input.InputSystem;
using UnityEngine;

public class Extinguisher : MonoBehaviour, IUsableObject
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _foam;
    [SerializeField] private float _foamDuration;
    [SerializeField] private Vector3 _sensorOffset;
    [SerializeField] private Vector3 _sensorExtents;

    private float _foamTime;
    private bool _usingFoam;

    public Collider Collider => _collider;

    public ExtinguisherAnimatorHandler AnimatorHandler { get; private set; }

    private Box FoamSensor => new Box
    (
        transform.position + transform.rotation * _sensorOffset,
        _sensorExtents,
        transform.rotation
    );

    private void Start()
    {
        AnimatorHandler = new(_animator);
        AnimatorHandler.Active.Set(false);

        _foamTime = 1f;
    }

    private void Update()
    {
        HandleFoamLevels();
    }

    private void FixedUpdate()
    {
        HandleSensor();
    }

    public void Use(PlayerHandler player)
    {
        player.EquipExtinguisher(this);
    }

    public void UseEquipped(InputButton button)
    {
        ButtonState state = button.State;

        if(state == ButtonState.Pressed && _foamTime > 0f)
        {
            _foam.Play();
            _usingFoam = true;
            return;
        }
        if (state == ButtonState.Released)
        {
            _foam.Stop();
            _usingFoam = false;
        }
    }

    private void HandleFoamLevels()
    {
        if(_usingFoam)
        {
            if (_foamTime == 1f)
                Persistence.Instance.Data.ExtintoresUsados++;

            _foamTime -= (1f / _foamDuration) * Time.deltaTime;

            if (_foamTime <= 0f)
                _foam.Stop();
        }
    }

    private void HandleSensor()
    {
        if (!_usingFoam)
            return;

        Collider[] colliders = Physics.OverlapBox(FoamSensor.Origin, FoamSensor.HalfExtents);

        foreach (Collider collider in colliders)
        {
            if(collider.transform.TryGetComponent(out Fire fire))
            {
                fire.ReceiveFoam();
                return;
            }
        }
    }

    [ContextMenu(nameof(ResetLife))]
    private void ResetLife()
    {
        _foamTime = 1f;
    }

    private void OnDrawGizmos()
    {
        if (_usingFoam)
        {
            Gizmos.color = Color.red;
            AideGizmo.DrawBox(FoamSensor);
        }
    }
}

