using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "StabAttack", menuName = "ActionTypes/Stab/StabAttack", order = 0)]
public class StabAttackSo : ScriptableObject
{
    [field:SerializeField] 
    public float AttackDuration { get; private set; }
    [field:SerializeField] 
    public float AttackDamages { get; private set; }
    [field:SerializeField] 
    public Vector3 SlashRotation { get; private set; }
    [field:SerializeField] 
    public AnimatorOverrideController AttackAnimatorOverride { get; private set; }

    [ShowAssetPreview(128, 128)] public GameObject collisionBox;

}
