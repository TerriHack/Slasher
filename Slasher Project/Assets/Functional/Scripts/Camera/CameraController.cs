using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [SerializeField] private AnimationCurve cameraTransitionCurve;
    
    private bool _transitionStarted;
    private float  _transitionCountdown;
    public Vector3 PreviousPosition {get; private set;}
    private Vector3 _currentTargetPosition;


    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;

        PreviousPosition = transform.position;
    }

    void Update()
    {
        CameraTransitionCountdown();
        CameraTransition();
    }

    public void StartCameraTransition(Vector3 targetPosition)
    {
        if (_transitionStarted)
        {
            ResetTransition();
            _currentTargetPosition = PreviousPosition;
            _transitionStarted = true;
            return;
        }
        _currentTargetPosition = targetPosition;
        PreviousPosition =  transform.position;
        _transitionStarted = true;
    }

    private void CameraTransition()
    {
        if(!_transitionStarted) return;
        transform.position = Vector3.Lerp(PreviousPosition, _currentTargetPosition, cameraTransitionCurve.Evaluate(_transitionCountdown));
    }

    private void ResetTransition()
    {
        _transitionCountdown = 0f;
        _transitionStarted = false;
        PreviousPosition = transform.position;
    }
    
    private void CameraTransitionCountdown()
    {
        if (!_transitionStarted)return;
        _transitionCountdown += Time.deltaTime;

        if (_transitionCountdown >= cameraTransitionCurve[cameraTransitionCurve.length-1].time)
        {
            ResetTransition();
        }
        
    }
}
