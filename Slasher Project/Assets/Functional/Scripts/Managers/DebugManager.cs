using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private Canvas debugCanvas;
    
    [BoxGroup("Input Debugging")]
    [SerializeField] private TextMeshProUGUI moveValueText;
    [BoxGroup("Input Debugging")]
    [SerializeField] private Image kickDebugImage;
    [BoxGroup("Input Debugging")]
    [SerializeField] private TextMeshProUGUI sprintValueText;
    [BoxGroup("Input Debugging")]
    [SerializeField] private TextMeshProUGUI stabValueText;
   
    [Button("Toggle Debug Canvas")]
    private void MethodTwo()
    {
        ToggleDebugCanvas();
    }
    
    private PlayerInputHandler playerInput;
    
    private bool kickCooldownStart = false;
    private float kickCountdown = 0f;
    
    private void  Start()
    {
        playerInput = PlayerInputHandler.Instance;
    }
    private void Update()
    {
        moveValueText.text = playerInput.MoveInput.ToString();
        sprintValueText.text = playerInput.SprintValue.ToString();
        stabValueText.text = playerInput.stabInput.ToString();
        
        KickCooldown();
    }

    private void ToggleDebugCanvas()
    {
        debugCanvas.enabled = !debugCanvas.enabled;
    }

    public void ToggleKickDebug()
    {
        kickDebugImage.enabled = !kickDebugImage.enabled;
        StartKickCooldown();
    }

    private void StartKickCooldown()
    {
        kickCooldownStart = true;
        kickCountdown = 0;
    }

    private void KickCooldown()
    {
        if (!kickCooldownStart) return;
        kickCountdown += Time.deltaTime;

        if (!(kickCountdown >= 1f)) return;
        ToggleKickDebug();
        kickCountdown = 0f;
        kickCooldownStart = false;
    }
}
