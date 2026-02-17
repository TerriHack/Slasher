using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Map Name References")] 
    [SerializeField] private string actionMapName = "Player";
    
    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string kick = "Kick";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string stab = "Stab";

    private InputAction moveAction;
    private InputAction kickAction;
    private InputAction sprintAction;
    private InputAction stabAction;
    
    public Vector2 MoveInput { get; private set; }
    public bool kickInput { get; private set; }
    public float SprintValue { get; private set; }
    public bool stabInput { get; private set; }
    
    public static PlayerInputHandler Instance { get; private set; }

    [Foldout("Events")]
    public UnityEvent onKick;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        kickAction = playerControls.FindActionMap(actionMapName).FindAction(kick);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        stabAction = playerControls.FindActionMap(actionMapName).FindAction(stab);
        
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;
        
        kickAction.performed += context => onKick.Invoke();
        
        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0f;
        
        stabAction.performed += context => stabInput = true;
        stabAction.canceled += context => stabInput = false;
    }
    
    private void OnEnable()
    {
        moveAction.Enable();
        kickAction.Enable();
        sprintAction.Enable();
        stabAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        kickAction.Disable();
        sprintAction.Disable();
        stabAction.Disable();
    }
}
