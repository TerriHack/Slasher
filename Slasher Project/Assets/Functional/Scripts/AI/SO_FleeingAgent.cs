using UnityEngine;

[CreateAssetMenu(fileName = "SO_FleeingAgent", menuName = "Scriptable Objects/SO_FleeingAgent")]
public class SO_FleeingAgent : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;

    [Header("Damage")] 
    public float timeToCombo;

    [Header("Bravery")] 
    public Vector2Int minMaxBravery;
}
