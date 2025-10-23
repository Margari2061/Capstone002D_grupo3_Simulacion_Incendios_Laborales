using UnityEngine;

public class UniformCase : MonoBehaviour, IUsableObject
{
    [SerializeField] private Material _shirtMaterial;
    [SerializeField] private Material _uniformMaterial;
    [SerializeField] private Material _orangeMaterial;
    [SerializeField] private Material _blackMaterial;
    [SerializeField] private MeshRenderer _renderer;

    public void Use(PlayerHandler player)
    {
        bool worn = player.WearUniform(_shirtMaterial, _uniformMaterial);
        Persistence.Instance.Data.UsoUniforme = worn;
        _renderer.material = worn switch
        {
            true => _blackMaterial,
            _ => _orangeMaterial
        };
    }
}