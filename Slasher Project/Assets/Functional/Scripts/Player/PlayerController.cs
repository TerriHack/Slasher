using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler _inputHandler;

    [SerializeField] private PlayerPresetSo _playerPresetSo;

    [field: SerializeField, ReadOnly, BoxGroup("Player Actions")]
    private bool _canMove;
    [field: SerializeField, ReadOnly, BoxGroup("Player Actions")]
    private bool _canStab;
    [field: SerializeField, ReadOnly, BoxGroup("Player Actions")]
    private bool _canSprint;
    [field: SerializeField, ReadOnly, BoxGroup("Player Actions")]
    private bool _canKick;
    
    [Foldout("Events")]
    public UnityEvent onStartWalking;
    [Foldout("Events")]
    public UnityEvent onStopWalking;
    
    private MoveSo _moveSo;
    private StabSo _stabSo;
    private SprintSo _sprintSo;
    private KickSo _kickSo;
    
    private enum PlayerState
    {
        Default,
        NoControl
    }
    
    [SerializeField] private PlayerState currentPlayerState = PlayerState.Default;

    private bool _isMoving;
    private float _movementAccelerationValue;
    private float _movementDecelerationValue;
    private Vector2 _lastMoveDir;
    
    private void Start()
    {
        _inputHandler = PlayerInputHandler.Instance;

        RegisterNewPlayerActions(_playerPresetSo);
    }
    private void FixedUpdate()
    {
        if (currentPlayerState == PlayerState.NoControl) return;
        
        //EXECUTE DEFAULT CONTROLLER
        transform.position += GetMovement();
        MovementCooldown();
    }

    public void RegisterNewPlayerActions(PlayerPresetSo newPlayerActions)
    {
        for (int i = 0; i < newPlayerActions.playerActions.Length-1; i++)
        {
            switch (newPlayerActions.playerActions[i])
            {
                case StabSo:
                    _canStab = true;
                    _stabSo = (StabSo)newPlayerActions.playerActions[i];
                    break;
                case MoveSo:
                    _canMove = true;
                    _moveSo = (MoveSo)newPlayerActions.playerActions[i];
                    break;
                case KickSo:
                    _canKick = true;
                    _kickSo = (KickSo)newPlayerActions.playerActions[i];
                    break;
                case SprintSo:
                    _canSprint = true;
                    _sprintSo = (SprintSo)newPlayerActions.playerActions[i];
                    break;
                default: Debug.LogWarning("Player action not recognised");
                    return;
            }
        }

        //Sets the new player preset
        _playerPresetSo = newPlayerActions;
    }
    private Vector3 GetMovement()
    {
        if (!_canMove) return Vector3.zero;
        
        if (_inputHandler.MoveInput != Vector2.zero) _lastMoveDir = _inputHandler.MoveInput;
        
        Vector3 movement = new Vector3(_lastMoveDir.x, 0, _lastMoveDir.y);
        
        //WIP ROTATION
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, MathF.Atan2(_lastMoveDir.x, _lastMoveDir.y) * Mathf.Rad2Deg, transform.eulerAngles.z);
        
        if (_isMoving)
        {
            onStartWalking.Invoke();
            return movement * _moveSo.AccelerationCurve.Evaluate(_movementAccelerationValue);
        }
        
        onStopWalking.Invoke();
        return movement * _moveSo.DecelerationCurve.Evaluate(_movementAccelerationValue);

    }

    private void MovementCooldown()
    {
        _isMoving = _inputHandler.MoveInput != Vector2.zero;
        if (_isMoving)
        {
            if(_movementAccelerationValue >= _moveSo.AccelerationCurve[_moveSo.AccelerationCurve.length-1].time) return;
            _movementAccelerationValue += Time.fixedDeltaTime;
        }
        else
        {
            if(_movementAccelerationValue <= 0) return;
            _movementAccelerationValue -= Time.fixedDeltaTime;
        }
    }
}
