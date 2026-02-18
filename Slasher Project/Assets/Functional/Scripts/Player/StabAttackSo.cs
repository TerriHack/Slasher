using UnityEngine;


[CreateAssetMenu(fileName = "StabAttack", menuName = "ActionTypes/Stab/StabAttack", order = 0)]
public class StabAttackSo : ScriptableObject
{
    [field:SerializeField] public float AttackDuration { get; private set; }
}
