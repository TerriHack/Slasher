using UnityEngine;
using Random = UnityEngine.Random;

public class KnockableObject : MonoBehaviour, IDamageable
{
    [SerializeField] private KnockableObjectSo knockableObjectSo;
    [SerializeField] private LayerMask knockedObjectLayer;
    
    private Rigidbody _rb;
    private float _r;
    private bool _isKnocked;
    private int _currentLifePoints;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _currentLifePoints = knockableObjectSo.LifePoints;
    }

    public void TakeDamage()
    {
        Eject(PlayerController.Instance.transform.position);
    }

    public void Eject(Vector3 hitPointPos)
    {
        Vector3 dir = new Vector3(transform.position.x - hitPointPos.x  , transform.position.y + 2f,
            transform.position.z - hitPointPos.z);
        
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Random.Range(-360,360),ref _r, 0.2f);
        transform.rotation = Quaternion.Euler(0, angle, 0);
        
        _rb.AddForce(dir * knockableObjectSo.EjectionStrength, ForceMode.Impulse);
        
        TurnObjectIntoKnockedObject();
        ManagePropsDestruction();
    }
    private void TurnObjectIntoKnockedObject()
    {
        gameObject.layer = LayerMask.NameToLayer("Knocked Object");
        _isKnocked = true;
    }

    private void ManagePropsDestruction()
    {
        _currentLifePoints--;

        if (_currentLifePoints <= 0)
        {
            Instantiate(knockableObjectSo.DestructionVFX, transform.position,transform.rotation);
            Destroy(gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(other == null || !_isKnocked || _rb.angularVelocity.magnitude < 1f) return;
        
        AiVictime victimeTouched;
        if (other.gameObject.GetComponent<AiVictime>() != null)
        {
            victimeTouched = other.gameObject.GetComponent<AiVictime>();
            victimeTouched.TakeDamage();
        }
    }
}
