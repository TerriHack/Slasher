using UnityEngine;

[CreateAssetMenu(fileName = "SO_PhaseZeroAgent", menuName = "Scriptable Objects/SO_PhaseZeroAgent")]
public class SO_PhaseZeroAgent : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;

    [Header("Phase 0")] 
    public float talkProbability;
    public float talkCooldown;
    public Vector2 minMaxDelayBeforeMoving;
    public Vector2 minMaxDelayBeforeFleeing;
    public float maxMoveRange;
}
