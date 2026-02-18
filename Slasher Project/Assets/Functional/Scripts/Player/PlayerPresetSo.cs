using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPreset", menuName = "PlayerPreset", order = 0)]
public class PlayerPresetSo: ScriptableObject
{
    public PlayerControllerActionTypeSo[] playerActions;
}
