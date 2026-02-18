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
    public void ActivateSprinting()
    {
        _animator.SetBool("isSprinting",true);
    }
    public void DeactivateSprinting()
    {
        _animator.SetBool("isSprinting",false);
    }
    
    public void ActivateStab()
    {
        _animator.SetLayerWeight(1,1);
        _animator.runtimeAnimatorController = PlayerController.Instance.Combo[PlayerController.Instance.ComboCounter].AttackAnimatorOverride;
        _animator.Play("Attack",1,0);
    }
}
