using UnityEngine;

public class Bubble : MonoBehaviour
{
    protected GameObject target;
    
    [SerializeField] protected Animation startAnimation;
    
    public virtual void InitBubble(GameObject target)
    {
        this.target = target;
        startAnimation.Play();
    }

    public virtual void Update()
    {
        var targetScreenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = targetScreenPos;
    }
}
