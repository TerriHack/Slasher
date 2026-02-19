using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Camera cam;
    [HideInInspector] public Vector2Int dimensions;
    
    private PlayerController player;

    private bool _transitionStarted;
    private float  _transitionCountdown;
    
    void Start()
    {
        cam = Camera.main;
        player = PlayerController.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        MoveCam();
    }

    private void MoveCam()
    {
        var direction = transform.position - player.transform.position;

        direction.y = 0;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.x = dimensions.x * (direction.x < 0 ? -1 : 1);
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
            direction.z = dimensions.y * (direction.z < 0 ? -1 : 1);
        }

        cam.transform.position += direction; // Pour Terri, à toi de Lerp ou de faire un truc stylé
    }

    private void TransitionCooldown()
    {
        
    }
}
