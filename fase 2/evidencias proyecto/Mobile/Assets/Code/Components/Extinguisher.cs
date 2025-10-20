using AideTool;
using AideTool.Input.InputSystem;
using UnityEngine;

public class Extinguisher : MonoBehaviour, IUsableObject
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _foam;

    public Collider Collider => _collider;

    public ExtinguisherAnimatorHandler AnimatorHandler { get; private set; }

    private void Start()
    {
        AnimatorHandler = new(_animator);
        AnimatorHandler.Active.Set(false);
    }

    public void Use(PlayerHandler player)
    {
        player.EquipExtinguisher(this);
    }

    public void UseEquipped(InputButton button)
    {
        ButtonState state = button.State;

        if(state == ButtonState.Pressed)
        {
            _foam.Play();
            return;
        }
        if (state == ButtonState.Released)
        {
            _foam.Stop();
        }
    }
}

