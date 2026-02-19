using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFeedbackBehavior : MonoBehaviour
{
    [FormerlySerializedAs("_animator")] [SerializeField] private Animator animator;
    [SerializeField] private Transform slashParent;
    [SerializeField] private GameObject basicSlash;
    
    public void ActivateWalking()
    {
        animator.SetBool("isWalking",true);
    }
    public void DeactivateWalking()
    {
        animator.SetBool("isWalking",false);
    }
    public void ActivateSprinting()
    {
        animator.SetBool("isSprinting",true);
    }
    public void DeactivateSprinting()
    {
        animator.SetBool("isSprinting",false);
    }
    
    public void ActivateStab()
    {
        animator.SetLayerWeight(1,1);
        animator.runtimeAnimatorController = PlayerController.Instance.Combo[PlayerController.Instance.ComboCounter].AttackAnimatorOverride;
        animator.Play("Attack",1,0);

        Vector3 spawnPos = new Vector3(slashParent.transform.position.x + PlayerInputHandler.Instance.MoveInput.x* 3f,
            slashParent.transform.position.y,
            slashParent.transform.position.z + PlayerInputHandler.Instance.MoveInput.y*3f);
        
        GameObject slash = Instantiate(basicSlash, spawnPos,slashParent.rotation);
        slash.transform.localEulerAngles = new Vector3(0, 0, 0);
        slash.transform.Rotate(PlayerController.Instance.Combo[PlayerController.Instance.ComboCounter].SlashRotation);
    }
}
