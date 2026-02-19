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
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationCurve rotationSpeed;

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
    
    public List<StabAttackSo> Combo { get; private set; }
    private float lastStabInputTime;
    private float lastComboEndTime;
    public int ComboCounter { get; private set; }
    
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

    private float r; 
    
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

    private void Update()
    {
        ExistStab();
    }
    
    private void FixedUpdate()
    {
        if (currentPlayerState == PlayerState.NoControl) return;
        
        //EXECUTE DEFAULT CONTROLLER
        transform.position += (GetMovement() + GetSprint()) * GetWallCollision();
        
        GetLastDirection();
        CharacterRotation();
        
        MovementValueCalculation();
        SprintValueCalculation();
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
                    InitializeNewStabCombo();
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
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, MathF.Atan2(_lastMoveDir.x, _lastMoveDir.y) * Mathf.Rad2Deg,ref r, 0.1f);
        transform.rotation = Quaternion.Euler(0, angle, 0);
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

    private int GetWallCollision()
    {
        RaycastHit raycastHit;
        
        Debug.DrawRay(transform.position, transform.forward*2, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 2f))
        {
            if (raycastHit.collider.tag == "Wall") return 0;
        }
        return 1;
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
        if (Time.time - lastComboEndTime > 0.5f && ComboCounter <= _stabSo.StabCombo.Length - 1)
        {
            CancelInvoke(nameof(EndCombo));

            if (Time.time - lastStabInputTime >= 0.5f)
            {
                onStabbing.Invoke();
                ManageStabCollisionBoxes(ComboCounter);
                ComboCounter++;
                lastStabInputTime = Time.time;

                if (ComboCounter > _stabSo.StabCombo.Length - 1)
                {
                    ComboCounter = 0;
                }
            }
        }
    }
    private void ExistStab()
    {
        if (Time.time - lastStabInputTime >= _stabSo.StabCombo[ComboCounter].AttackDuration)
        {
            ManageStabCollisionBoxes();
        }
        
        if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.9f &&
            animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
        {
            Invoke(nameof(EndCombo),1);
        }
    }
    private void EndCombo()
    {
        ComboCounter = 0;
        lastComboEndTime = Time.time;
        
        onStopStabbing.Invoke();
    }
    
    private void InitializeNewStabCollisionBoxes()
    {
        if(!_canStab) return;
        stabAttackCollisionBoxes = new List<GameObject>();
        foreach (var t in _stabSo.StabCombo)
        {
            GameObject box = Instantiate(t.collisionBox, collisionBoxParent);
            stabAttackCollisionBoxes.Add(box);
            box.SetActive(false);
        }
    }
    private void InitializeNewStabCombo()
    {
        if(!_canStab) return;
        Combo = new List<StabAttackSo>();
        foreach (var t in _stabSo.StabCombo)
        {
            Combo.Add(t);
        }
    }

    private void ManageStabCollisionBoxes(int boxIndex)
    {
        for (int i = 0; i < stabAttackCollisionBoxes.Count; i++)
        {
            if (i == boxIndex)
            {
                stabAttackCollisionBoxes[i].SetActive(true);
            }
            else
            {
                stabAttackCollisionBoxes[i].SetActive(false);
            }
        }
    }
    private void ManageStabCollisionBoxes()
    {
        foreach (var boxIndex in stabAttackCollisionBoxes)
        {
            boxIndex.SetActive(false);
        }
    }
}
