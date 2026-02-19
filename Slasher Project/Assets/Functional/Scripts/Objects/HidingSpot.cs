using UnityEngine;

public class HidingSpot : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxCapacity;
    private int currentPeopleInIt;

    public bool TryHiding()
    {
        if (HasRoom())
        {
            currentPeopleInIt++;
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
        Destroy(gameObject);
    }
}
