using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AiVctime : MonoBehaviour
{
    [SerializeField] private SO_PhaseZeroAgent baseData;
    [SerializeField] private SO_FleeingAgent fleeData;

    private NavMeshAgent agent;

    private float moveSpeedWalk;
    private float moveSpeedRun;
    private int bravery;
    private float talkProbability;
    private Vector2 minMaxDelayBeforeMoving;
    private float delayBeforeFleeing;
    private float maxMoveRange;

    private float timerBeforeMoving;
    private Vector3 startPos;
    private Vector3 targetPos;

    private float talkCooldown;
    private float talkTimer;

    private float timeToCombo;
    private float damageTimer;
    private bool hasTakenDamage;
    
    #region Init
    private void Start()
    {
        InitBaseData();
        InitFleeData();
        InitTimers();
        
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = moveSpeedWalk;
    }

    private void InitBaseData()
    {
        if (baseData == null)
        {
            Debug.LogError("PAS DE DATA : ABORT");
            return;
        }

        moveSpeedWalk = baseData.moveSpeed;
        talkProbability = baseData.talkProbability;
        minMaxDelayBeforeMoving = baseData.minMaxDelayBeforeMoving;
        delayBeforeFleeing = Random.Range(baseData.minMaxDelayBeforeFleeing.x, baseData.minMaxDelayBeforeFleeing.y);
        maxMoveRange = baseData.maxMoveRange;
        talkCooldown = baseData.talkCooldown;
    }

    private void InitFleeData()
    {
        if (fleeData == null)
        {
            Debug.LogError("PAS DE DATA : ABORT");
            return;
        }
        
        bravery = Random.Range(fleeData.minMaxBravery.x, fleeData.minMaxBravery.y);
        timeToCombo = fleeData.timeToCombo;
    }

    private void InitTimers()
    {
        timerBeforeMoving = Random.Range(minMaxDelayBeforeMoving.x, minMaxDelayBeforeMoving.y);
    }
    
    #endregion

    private void Update()
    {
        PhaseZeroCheckToMove();
        CheckDeathFromDamage();
    }

    #region Phase Zero
    private void PhaseZeroCheckToMove()
    {
        if (maxMoveRange <= 0) return;
        
        timerBeforeMoving -= Time.deltaTime;
        if (timerBeforeMoving <= 0)
        {
            if (targetPos != Vector3.zero && Vector3.Distance(transform.position, targetPos) > 2) //Empêche les double déplacements tant qu'il n'a pas atteint sa destination
            {
                timerBeforeMoving = Random.Range(minMaxDelayBeforeMoving.x, minMaxDelayBeforeMoving.y);
                return;
            }
            
            var direction = Random.insideUnitCircle.normalized;
            
            var target = startPos + new Vector3(direction.x, 0, direction.y) * maxMoveRange;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 50f, NavMesh.AllAreas))
            {
                targetPos = hit.position;
                agent.SetDestination(targetPos);
            }

            timerBeforeMoving = Random.Range(minMaxDelayBeforeMoving.x, minMaxDelayBeforeMoving.y);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        if (talkTimer > 0) return;

        if (Random.Range(0, 100) > talkProbability) return;
        
        SpawnDialogueBubble();
    }

    private void SpawnDialogueBubble()
    {
        if (DialogueManager.Instance.SpawnDialogueBubble(gameObject, 0))
        {
            talkTimer = talkCooldown;
            Debug.Log("TALK");
        }
    }
    
    #endregion

    #region Damages
    
    public void TakeDamage()
    {
        damageTimer = timeToCombo;
        hasTakenDamage = true;
    }

    private void CheckDeathFromDamage()
    {
        if (!hasTakenDamage) return;
        
        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0) Die();
    }

    public void Die()
    {
        agent.enabled = false;
        DieFeedbacks();
    }

    private void DieFeedbacks()
    {
        transform.LookAt(transform.position + Vector3.up * -5);
    }
    
    #endregion

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TakeDamage();
        }
    }
}
