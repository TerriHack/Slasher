using UnityEngine;

public class Bubble : MonoBehaviour
{
    protected GameObject target;
    
    public virtual void InitBubble(GameObject target)
    {
        this.target = target;
    }

    public virtual void Update()
    {
        var targetScreenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = targetScreenPos;
    }
}
