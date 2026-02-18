using UnityEngine;

public class PlayerFeedbackBehavior : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void ActivateWalking()
    {
        _animator.SetBool("isWalking",true);
    }
    public void DeactivateWalking()
    {
        _animator.SetBool("isWalking",false);
    }
}
