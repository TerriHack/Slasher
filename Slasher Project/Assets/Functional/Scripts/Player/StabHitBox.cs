using UnityEngine;

public class StabHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable idamageable))
        {
            idamageable.TakeDamage();
        }
    }
}
