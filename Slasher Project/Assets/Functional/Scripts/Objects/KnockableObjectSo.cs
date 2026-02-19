using UnityEngine;


[CreateAssetMenu(fileName = "KnockableObjects", menuName = "KnockableObjects", order = 0)]
public class KnockableObjectSo : PlayerControllerActionTypeSo

{
    [field: SerializeField] public float EjectionStrength { get; private set; } = 5f;
    [field: SerializeField] public int LifePoints { get; private set; } = 3;
    [field: SerializeField] public GameObject DestructionVFX { get; private set; }

}
