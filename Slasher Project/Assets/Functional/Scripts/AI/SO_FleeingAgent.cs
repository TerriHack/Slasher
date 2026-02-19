using UnityEngine;

[CreateAssetMenu(fileName = "SO_FleeingAgent", menuName = "Scriptable Objects/SO_FleeingAgent")]
public class SO_FleeingAgent : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;
    public float fleeRadius;

    [Header("Damage")] 
    public float timeToCombo;

    [Header("Bravery")] 
    public Vector2Int minMaxBravery;
    public float scareRange;
    public float regainBraverySpeed;
    public float minimumBraveryForWeapon;

    [Header("Hiding")] 
    public float hidingSpotRange;
    public float maxBraveryToHide;

    [Header("Weapon")] 
    public float weaponSearchRange;
    public float delayBeforeShooting;
}
