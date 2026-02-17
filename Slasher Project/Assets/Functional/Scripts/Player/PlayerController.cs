using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler inputHandler;

    private enum PlayerState
    {
        Default,
        NoControl
    }
    
    [SerializeField] private PlayerState currentPlayerState = PlayerState.Default;
    
    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
    }
    private void FixedUpdate()
    {
        if (currentPlayerState == PlayerState.NoControl) return;
        
        //EXECUTE DEFAULT CONTROLLER
    }
    
    private Vector3 GetMovement()
    {
        Vector3 dir = inputHandler.MoveInput.normalized;
        return dir; //Vector3 move = new Vector3(dir * )
    }
}
