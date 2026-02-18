using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AiVictime : MonoBehaviour
{
    [SerializeField] private SO_PhaseZeroAgent baseData;
    [SerializeField] private SO_FleeingAgent fleeData;

    private NavMeshAgent agent;

    private bool phaseZeroIsEnded;

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

    private GameObject player;
    
    #region Init
    private void Start()
    {
        InitBaseData();
        InitFleeData();
        InitTimers();
        
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = moveSpeedWalk;
        agent.acceleration = baseData.acceleration;
        
        GameManager.Instance.startFleeingPhase += StartFleeing;
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
        
        moveSpeedRun = fleeData.moveSpeed;
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
        if (phaseZeroIsEnded) return;
        
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
        if (phaseZeroIsEnded) return;
        
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
        UnsubscribeToEverything();
        DisableThatScript();

        if (!phaseZeroIsEnded)
        {
            GameManager.Instance.FirstBlood();
        }
    }

    private void DieFeedbacks()
    {
        transform.LookAt(transform.position + Vector3.up * -5);
    }

    private void DisableThatScript()
    {
        enabled = false;
    }

    private void UnsubscribeToEverything()
    {
        GameManager.Instance.startFleeingPhase -= StartFleeing;
    }
    
    #endregion
    
    #region Flee

    private void StartFleeing()
    {
        phaseZeroIsEnded = true;
        agent.enabled = false;
        agent.speed = 0;
        player = GameManager.Instance.GetPlayer();
        
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        DialogueManager.Instance.SpawnShockedBubble(gameObject);
        
        StartCoroutine(Flee());
    }

    private IEnumerator Flee()
    {
        yield return new WaitForSeconds(delayBeforeFleeing);
        agent.enabled = true;
        agent.acceleration = fleeData.acceleration;
        agent.speed = moveSpeedRun;
        agent.SetDestination(transform.position + (transform.position - player.transform.position).normalized * 10f);
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
