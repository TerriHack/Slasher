using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxCapacity;
    private int currentPeopleInIt;
    private List<GameObject> aiInIt = new List<GameObject>();

    public bool TryHiding(GameObject ai)
    {
        if (HasRoom())
        {
            currentPeopleInIt++;
            aiInIt.Add(ai);
            return true;
        }

        else
        {
            return false;
        }
    }

    public bool HasRoom()
    {
        return currentPeopleInIt < maxCapacity;
    }

    public void TakeDamage()
    {
        foreach (GameObject ai in aiInIt)
        {
            ai.transform.position =  transform.position + (Vector3)Random.insideUnitCircle * 5f;
            ai.GetComponent<AiVictime>().LeaveHidingSpot();
        }
        
        Destroy(gameObject);
    }
}
