using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController  Instance;
    
    private PlayerInputHandler _inputHandler;

    [SerializeField] private PlayerPresetSo playerPresetSo;
    [SerializeField] private Transform collisionBoxParent;

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
    [Foldout("Events")]
    public UnityEvent onStartSprinting;
    [Foldout("Events")]
    public UnityEvent onStopSprinting;
    [Foldout("Events")]
    public UnityEvent onStabbing;
    [Foldout("Events")]
    public UnityEvent onStopStabbing;
    
    private MoveSo _moveSo;
    private StabSo _stabSo;
    private SprintSo _sprintSo;
    private KickSo _kickSo;

    private List<GameObject> stabAttackCollisionBoxes;
    
    private enum PlayerState
    {
        Default,
        NoControl
    }
    
    [SerializeField] private PlayerState currentPlayerState = PlayerState.Default;

    private bool _isMoving;
    private float _movementValue;
    private Vector2 _lastMoveDir;

    private bool _isSprinting;
    private float _sprintValue;
    
    private bool _isStabbing;
    private bool _canAttack = true;
    private float _stabCountdown;
    public int currentStabAttackIndex;
    public bool _stabAttack0Launched;
    public bool _stabAttack1Launched;
    public bool _stabAttack2Launched;
    
    
    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        _inputHandler = PlayerInputHandler.Instance;

        RegisterNewPlayerActions(playerPresetSo);
    }
    private void FixedUpdate()
    {
        if (currentPlayerState == PlayerState.NoControl) return;
        
        //EXECUTE DEFAULT CONTROLLER
        transform.position += GetMovement() + GetSprint();
        
        GetLastDirection();
        CharacterRotation();
        
        MovementValueCalculation();
        SprintValueCalculation();
        
        ManageStabCooldown();
    }

    public void RegisterNewPlayerActions(PlayerPresetSo newPlayerActions)
    {
        for (int i = 0; i < newPlayerActions.playerActions.Length; i++)
        {
            switch (newPlayerActions.playerActions[i])
            {
                case StabSo:
                    _canStab = true;
                    _stabSo = (StabSo)newPlayerActions.playerActions[i];
                    InitializeNewStabCollisionBoxes();
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
        playerPresetSo = newPlayerActions;
    }

    private void GetLastDirection()
    {
        if (_inputHandler.MoveInput != Vector2.zero) _lastMoveDir = _inputHandler.MoveInput;
    }
    private void CharacterRotation()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, MathF.Atan2(_lastMoveDir.x, _lastMoveDir.y) * Mathf.Rad2Deg, transform.eulerAngles.z);
    }
    
    private Vector3 GetMovement()
    {
        if (!_canMove) return Vector3.zero;
        
        if (_inputHandler.MoveInput != Vector2.zero) _lastMoveDir = _inputHandler.MoveInput;
        
        Vector3 movement = new Vector3(_lastMoveDir.x, 0, _lastMoveDir.y);
        
        if (_isMoving)
        {
            onStartWalking.Invoke();
            return movement * _moveSo.AccelerationCurve.Evaluate(_movementValue);
        }
        
        onStopWalking.Invoke();
        return movement * _moveSo.DecelerationCurve.Evaluate(_movementValue);

    }
    private void MovementValueCalculation()
    {
        if(!_canMove)return;
        _isMoving = _inputHandler.MoveInput != Vector2.zero;
        if (_isMoving)
        {
            if(_movementValue >= _moveSo.AccelerationCurve[_moveSo.AccelerationCurve.length-1].time) return;
            _movementValue += Time.fixedDeltaTime;
        }
        else
        {
            if(_movementValue <= 0) return;
            _movementValue -= Time.fixedDeltaTime;
        }
    }
    
    private Vector3 GetSprint()
    {
        if (!_canMove) return Vector3.zero;
        
        Vector3 movement = new Vector3(_lastMoveDir.x, 0, _lastMoveDir.y);
        
        if (_isSprinting)
        {
            onStartSprinting.Invoke();
            return movement * _sprintSo.AccelerationCurve.Evaluate(_sprintValue);
        }
        
        onStopSprinting.Invoke();
        return movement * _sprintSo.DecelerationCurve.Evaluate(_sprintValue);
    }
    private void SprintValueCalculation()
    {
        if(!_canSprint) return;
        
        _isSprinting = _inputHandler.MoveInput != Vector2.zero && _inputHandler.SprintValue != 0f;
        
        if (_isSprinting)
        {
            if(_sprintValue >= _sprintSo.AccelerationCurve[_sprintSo.AccelerationCurve.length-1].time) return;
            _sprintValue += Time.fixedDeltaTime;
        }
        else
        {
            if(_sprintValue <= 0) return;
            _sprintValue -= Time.fixedDeltaTime;
        }
    }

    public void Stab()
    {
        if(!_canStab) return;

        if (!_stabAttack0Launched && _canAttack)
        {
            _stabAttack0Launched = true;
            currentStabAttackIndex = 0;
            StartStabCooldown();
            ManageStabCollisionBoxes(true,currentStabAttackIndex);
            onStabbing.Invoke();
            
            return;
        }
        if (!_stabAttack1Launched && _canAttack)
        {
            _stabAttack1Launched = true;
            currentStabAttackIndex = 1;
            StartStabCooldown();
            ManageStabCollisionBoxes(true,currentStabAttackIndex);
            onStabbing.Invoke();
            return;
        }
        if (!_stabAttack2Launched && _canAttack)
        {
            _stabAttack2Launched = true;
            currentStabAttackIndex = 2;
            StartStabCooldown();
            ManageStabCollisionBoxes(true,currentStabAttackIndex);
            onStabbing.Invoke();
            return;
        }
        
        ResetStabAttack();
        ResetStabCooldown();
    }

    private void ResetStabAttack()
    {
        _stabAttack0Launched = false;
        _stabAttack1Launched = false;
        _stabAttack2Launched = false;
    }

    private void StartStabCooldown()
    {
        _isStabbing = true;
        _stabCountdown = 0f;
    }
    private void ResetStabCooldown()
    {
        _isStabbing = false;
        _stabCountdown = 0f;
    }
    private void ManageStabCooldown()
    {
        if(!_isStabbing) return;
        _stabCountdown += Time.fixedDeltaTime;

        if (_stabCountdown >= _stabSo.StabCombo[currentStabAttackIndex].AttackDuration)
        {
            _canAttack = true;
            ManageStabCollisionBoxes(false,currentStabAttackIndex);
        }
        else
        {
            _canAttack = false;
        }
        
        if (_stabCountdown >= _stabSo.StabCombo[currentStabAttackIndex].AttackDuration + _stabSo.DurationBeforeReset)
        {
            _canAttack = true;
            ResetStabCooldown(); 
            ResetStabAttack();
            onStopStabbing.Invoke();
        }
    }
    
    
    private void InitializeNewStabCollisionBoxes()
    {
        if(!_canStab) return;
        stabAttackCollisionBoxes = new List<GameObject>();
        foreach (var t in _stabSo.StabCombo)
        {
            Debug.Log(t);
            GameObject box = Instantiate(t.collisionBox, collisionBoxParent);
            stabAttackCollisionBoxes.Add(box);
            box.SetActive(false);
        }
    }

    private void ManageStabCollisionBoxes(bool active, int boxIndex)
    {
        stabAttackCollisionBoxes[boxIndex].SetActive(active);
    }
}
