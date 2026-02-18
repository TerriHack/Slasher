using UnityEngine;


[CreateAssetMenu(fileName = "SprintActionType", menuName = "ActionTypes/Sprint", order = 0)]
public class SprintSo : PlayerControllerActionTypeSo
{
    [field:SerializeField] public AnimationCurve AccelerationCurve { get; private set; }
    [field:SerializeField] public AnimationCurve DecelerationCurve { get; private set; }
}
