using UnityEngine;


[CreateAssetMenu(fileName = "MoveActionType", menuName = "ActionTypes/Move", order = 0)]
public class MoveSo : PlayerControllerActionTypeSo
{
    [field:SerializeField] public AnimationCurve AccelerationCurve { get; private set; }
    [field:SerializeField] public AnimationCurve DecelerationCurve { get; private set; }
}
