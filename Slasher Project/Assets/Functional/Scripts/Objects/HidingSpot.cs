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
        for (int i = aiInIt.Count - 1; i >= 0; i--)
        {
            var ai = aiInIt[i];
            ai.transform.position =  transform.position + (Vector3)Random.insideUnitCircle * 5f;
            ai.GetComponent<AiVictime>().LeaveHidingSpot();
        }
        
        Destroy(gameObject);
    }

    public void LeaveHidingSpot(GameObject ai)
    {
        aiInIt.Remove(ai);
    }
}
