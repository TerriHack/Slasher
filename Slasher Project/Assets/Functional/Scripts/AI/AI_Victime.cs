using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class AiVictime : MonoBehaviour, IDamageable
{
    [SerializeField] private SO_PhaseZeroAgent baseData;
    [SerializeField] private SO_FleeingAgent fleeData;
    [SerializeField] private ParticleSystem bloodVFX;

    private NavMeshAgent agent;

    private bool phaseZeroIsEnded;

    private float moveSpeedWalk;
    private float moveSpeedRun;
    private int bravery;
    private float talkProbability;
    private Vector2 minMaxDelayBeforeMoving;
    private float delayBeforeFleeing;
    private float maxMoveRange;
    private float fleeRadius;

    private float timerBeforeMoving;
    private Vector3 startPos;
    private Vector3 targetPos;

    private float talkCooldown;
    private float talkTimer;

    private float timeToCombo;
    private float damageTimer;
    private bool hasTakenDamage;

    private bool needToSelectAction;

    private float hidingSpotRange;
    private bool tryToHide;
    public GameObject targetedSpot;
    private bool isHidden;

    private bool isInterrupted;
    
    private GameObject player;

    [SerializeField] private GameObject bloodSplatter;
    
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
        fleeRadius = fleeData.fleeRadius;
        hidingSpotRange = fleeData.hidingSpotRange;
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
        
        ChooseAction();
        CheckIfDestinationIsReached();
    }

    private void ChooseAction()
    {
        if (!needToSelectAction) return;

        if (isInterrupted) return;

        // Courageux = Arme
        
        if (Vector3.Distance(player.transform.position, transform.position) <= fleeRadius)
        {
            Flee();
        }
        else
        {
            LookForHidingSpot();
        }
        
        needToSelectAction = false;
    }

    private void CheckIfDestinationIsReached()
    {
        if (!phaseZeroIsEnded) return;

        if (isHidden) return;

        if (Vector3.Distance(transform.position, targetPos) <= 2f)
        {
            if (tryToHide)
            {
                if (targetedSpot.GetComponent<HidingSpot>().TryHiding(gameObject))
                {
                    Hide();
                    return;
                }
            }
            
            needToSelectAction = true;
        }
    }

    public void InterruptAction(bool definitive)
    {
        if (!phaseZeroIsEnded) return;

        MoveTo(transform.position);
        
        tryToHide = false;

        if (!definitive)
        {
            needToSelectAction = true;
        }
        else
        {
            isInterrupted = true;
        }
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
                MoveTo(hit.position);
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
        bloodVFX.Play();
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
        
        var blood = Instantiate(bloodSplatter, transform.position + Vector3.down, transform.rotation);
        blood.GetComponent<DecalProjector>().size = Random.Range(0.8f, 4f) * Vector3.one;
        blood.transform.RotateAround(Vector3.up, Random.Range(0, 360));
    }

    private void DisableThatScript()
    {
        Destroy(this);
    }

    private void UnsubscribeToEverything()
    {
        GameManager.Instance.startFleeingPhase -= StartFleeing;
    }
    
    #endregion
    
    #region Phase Zero Ending

    private void StartFleeing()
    {
        phaseZeroIsEnded = true;
        agent.enabled = false;
        agent.speed = 0;
        player = GameManager.Instance.GetPlayer();
        
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        DialogueManager.Instance.SpawnShockedBubble(gameObject);
        
        StartCoroutine(WaitBeforeFlee());
    }

    private IEnumerator WaitBeforeFlee()
    {
        yield return new WaitForSeconds(delayBeforeFleeing);
        agent.enabled = true;
        agent.acceleration = fleeData.acceleration;
        agent.speed = moveSpeedRun;
        needToSelectAction = true;
    }
    
    #endregion
    
    #region Flee
    
    private void Flee()
    {
        Debug.Log("FLEE");
        var direction = transform.position - player.transform.position;
        direction.y = 0;
        var randomDirection = (Quaternion.AngleAxis(Random.Range(-60, 60), Vector3.up) * direction).normalized;
        if (NavMesh.SamplePosition(transform.position + randomDirection * 10f, out NavMeshHit hit, 10f,
                NavMesh.AllAreas))
        {
            MoveTo(hit.position);
        }
        else
        {
            LookForHidingSpot();
        }
    }
    
    #endregion
     
    #region Hide

    private void LookForHidingSpot()
    {
        Debug.Log("LOOK FOR HIDING SPOT");

        Collider[] hits = Physics.OverlapSphere(transform.position, hidingSpotRange);
        List<GameObject> possibleHideSpots = new List<GameObject>();

        foreach (var hit in hits)
        {
            if (hit.GetComponent<Collider>().CompareTag("Hiding"))
            {
                if(hit.gameObject.GetComponent<HidingSpot>().HasRoom()) possibleHideSpots.Add(hit.gameObject);
            }
        }

        if (possibleHideSpots.Count <= 0)
        {
            Debug.Log("NO HIDING SPOT");
            Flee();
        }
        else
        {
            GoToHidingSpot(possibleHideSpots[Random.Range(0, possibleHideSpots.Count)]);
        }
    }

    private void GoToHidingSpot(GameObject hiddenSpot)
    {
        if (NavMesh.SamplePosition(hiddenSpot.transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            targetedSpot = hiddenSpot;
            
            MoveTo(hit.position);
            
            tryToHide = true;
            Debug.Log("Hiding Spot Found");
        }
    }

    private void Hide()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        agent.enabled = false;
        isHidden = true;
    }

    public void LeaveHidingSpot()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        agent.enabled = true;
        isHidden = false;
        InterruptAction(false);
    }
    
    #endregion

    private void MoveTo(Vector3 pos)
    {
        targetPos = pos;
        agent.SetDestination(targetPos);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, targetPos);
    }
}
