using UnityEngine;

public class StabHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AiVictime victimeTouched;
        if (other.GetComponent<AiVictime>() != null)
        {
            victimeTouched = other.GetComponent<AiVictime>();
            victimeTouched.TakeDamage();
        }

        KnockableObject touchedObject;
        if (other.GetComponent<KnockableObject>() != null)
        {
            touchedObject = other.GetComponent<KnockableObject>();
            touchedObject.Eject(PlayerController.Instance.transform.position);
        }
    }
}
