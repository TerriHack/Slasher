using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "StabActionType", menuName = "ActionTypes/Stab", order = 0)]
public class StabSo : PlayerControllerActionTypeSo
{
    [field:SerializeField] public float DurationBeforeReset { get; private set; }
    [field:SerializeField, ReorderableList] public StabAttackSo[] StabCombo { get; private set; }
}
